using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Data.Models;
using Maxima.Cliente.Omie.Domain.Api.Parameters;
using Maxima.Cliente.Omie.Domain.Api.Requests;
using Maxima.Cliente.Omie.Domain.Api.Response;
using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Cliente.Omie.Domain.Enums;
using Maxima.Cliente.Omie.Domain.Interfaces.Api;
using Maxima.Cliente.Omie.Domain.Utils;
using Maxima.Net.SDK.Integracao.Api;
using Maxima.Net.SDK.Integracao.Dto;
using Maxima.Net.SDK.Integracao.Entidades;
using Maxima.Net.SDK.Integracao.Utils;
using Microsoft.EntityFrameworkCore;

namespace Maxima.Cliente.Omie.Domain.Service
{
    public class FilialApiOmie : IFilialApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;

        public FilialApiOmie(OmieContext context, IMapper mapper, MaximaIntegracao maximaIntegracao)
        {
            this.dbContext = context;
            this.mapper = mapper;
            apiMaxima = maximaIntegracao;
        }

        public async Task EnviarFiliais(CancellationToken token)
        {
            LogApi log = new("Filiais");

            try
            {
                var filiaisBd = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.FILIAIS).AsNoTracking().ToListAsync();
                var processados = new List<string>();
                var pagina = 1;
                long totalPaginas;
                long countOmie;

                do
                {
                    var listaIncluir = new List<FilialMaxima>();
                    var listaAlterar = new List<FilialMaxima>();

                    var request = new RequestOmie
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "ListarEmpresas",
                        Params = new List<Param>() {
                                    new Param() {
                                        Pagina = pagina
                                    }
                        }
                    };

                    var result = await ApiUtilsMaxima.RequisicaoAsync<ResponseFilialOmie, RequestOmie>(FilialOmie.UrlApi, HttpMethod.Post, request);

                    totalPaginas = result.TotalDePaginas;
                    countOmie = result.TotalDeRegistros;

                    foreach (var filiais in result.Filiais)
                    {
                        var mapFiliais = mapper.Map<FilialMaxima>(filiais);

                        if (filiaisBd.Any(x => x.Chave == mapFiliais.CodigoFilial && x.Valor != mapFiliais.Hash))
                        {
                            listaAlterar.Add(mapFiliais);
                        }
                        else if (!filiaisBd.Any(x => x.Chave == mapFiliais.CodigoFilial))
                        {
                            listaIncluir.Add(mapFiliais);
                        }
                    }

                    if (listaIncluir.Any())
                    {
                        ResponseApiMaxima<FilialMaxima> retornoApiMaxima = await apiMaxima.IncluirFilial(listaIncluir);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoFilial.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                ControleDadosModel filiaisModel = new()
                                {
                                    Tabela = ControleDadosEnum.FILIAIS,
                                    Chave = item.CodigoFilial,
                                    Valor = item.Hash
                                };
                                dbContext.ControleDadosModels.Add(filiaisModel);
                            }
                            await dbContext.SaveChangesAsync();
                            log.InserirOk(pagina, totalPaginas, retornoApiMaxima.ItensInserido.Count());

                            if (retornoApiMaxima.ErrosValidacao.Any())
                                log.InserirErro(pagina, totalPaginas, retornoApiMaxima.TotalItensNaoInserido, retornoApiMaxima.ErrosValidacaoFormatado);
                        }
                        else
                        {
                            log.InserirErro(pagina, totalPaginas, listaIncluir.Count(), retornoApiMaxima.ErrosValidacaoFormatado);
                        }
                    }

                    if (listaAlterar.Any())
                    {
                        ResponseApiMaxima<FilialMaxima> retornoApiMaxima = await apiMaxima.AlterarFilial(listaAlterar);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoFilial.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                var filiaisModel = filiaisBd.Where(c => c.Chave == item.CodigoFilial).FirstOrDefault();
                                if (filiaisModel != null)
                                {
                                    filiaisModel.Valor = item.Hash;
                                    dbContext.Update(filiaisModel);
                                }
                            }
                            await dbContext.SaveChangesAsync();
                            log.InserirOk(pagina, totalPaginas, retornoApiMaxima.ItensInserido.Count());

                            if (retornoApiMaxima.ErrosValidacao.Any())
                                log.InserirErro(pagina, totalPaginas, retornoApiMaxima.TotalItensNaoInserido, retornoApiMaxima.ErrosValidacaoFormatado);
                        }
                        else
                        {
                            log.AlterarErro(pagina, totalPaginas, listaAlterar.Count, retornoApiMaxima.ErrosValidacaoFormatado);
                        }
                    }

                    if (!listaAlterar.Any() && !listaIncluir.Any())
                        log.NenhumRegistroAlterado(pagina, totalPaginas);

                    pagina++;
                } while (pagina <= totalPaginas);

                var filialList = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.FILIAIS)
                    .AsNoTracking()
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(filialList)
                    .ToList();

                var filiaisRemove = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.FILIAIS)
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave))
                    .ToListAsync();

                if (excluidos.Any())
                {
                    var listaExclusao = filiaisRemove.Select(x => x.Valor);
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarFilial(listaExclusao.ToArray());
                    if (retornoApiMaxima.Sucesso)
                    {
                        dbContext.ControleDadosModels.RemoveRange(filiaisRemove);
                        await dbContext.SaveChangesAsync();
                        log.ExcluirOk(listaExclusao.Count());
                    }
                    else
                    {
                        dbContext.ChangeTracker.Clear();
                        log.ExcluirErro(listaExclusao.Count(), retornoApiMaxima.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                log.GlobalError(ex.Message);
                throw;
            }
        }
    }
}
