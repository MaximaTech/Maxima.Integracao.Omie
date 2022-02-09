using AutoMapper;
using System.Threading;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Data.Models;
using Maxima.Cliente.Omie.Domain.Api.Response;
using Maxima.Cliente.Omie.Domain.Api.Parameters;
using Maxima.Cliente.Omie.Domain.Api.Requests;
using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Cliente.Omie.Domain.Enums;
using Maxima.Cliente.Omie.Domain.Interfaces.Api;
using Maxima.Cliente.Omie.Domain.Utils;
using Maxima.Net.SDK.Integracao.Api;
using Maxima.Net.SDK.Integracao.Dto;
using Maxima.Net.SDK.Integracao.Entidades;
using Maxima.Net.SDK.Integracao.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Maxima.Cliente.Omie.Domain.Service
{
    public class FamiliaApiOmie : IFamiliaApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;
        public FamiliaApiOmie(OmieContext context, IMapper mappe, MaximaIntegracao maximaIntegracao)
        {
            dbContext = context;
            mapper = mappe;
            this.apiMaxima = maximaIntegracao;
        }
        public async Task ListarFamilias(CancellationToken token)
        {
            LogApi log = new("Familias");

            try
            {

                var familiaDb = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.FAMILIAS).AsNoTracking().ToListAsync();
                var pagina = 1;
                long totalPaginas;
                long countOmie;
                var processados = new List<string>();
                do
                {
                    var listaIncluir = new List<FamiliaMaxima>();
                    var listaAlterar = new List<FamiliaMaxima>();
                    var request = new RequestOmie
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "PesquisarFamilias",
                        Params = new List<Param> { new Param() { Pagina = pagina, } }
                    };
                    var resultApi = await ApiUtilsMaxima.RequisicaoAsync<ResponseFamiliaOmie, RequestOmie>(FamiliaOmie.UrlApi, HttpMethod.Post, request);

                    totalPaginas = resultApi.TotalDePaginas;
                    countOmie = resultApi.TotalDeRegistros;

                    foreach (var familia in resultApi.Familias)
                    {

                        var mapFamilia = mapper.Map<FamiliaMaxima>(familia);

                        if (familiaDb.Any() && familiaDb.Any(x => x.Chave == mapFamilia.CodigoFamilia.ToString() && x.Valor != mapFamilia.Hash))
                        {
                            listaAlterar.Add(mapFamilia);
                        }
                        else if (!familiaDb.Any(x => x.Chave == mapFamilia.CodigoFamilia.ToString()))
                        {
                            listaIncluir.Add(mapFamilia);
                        }
                    }

                    if (listaIncluir.Any())
                    {
                        ResponseApiMaxima<FamiliaMaxima> retornoApiMaxima = await apiMaxima.IncluirFamilias(listaIncluir);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoFamilia.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                ControleDadosModel familiaModel = new()
                                {
                                    Tabela = ControleDadosEnum.FAMILIAS,
                                    Chave = item.CodigoFamilia.ToString(),
                                    Valor = item.Hash
                                };
                                dbContext.ControleDadosModels.Add(familiaModel);
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
                        ResponseApiMaxima<FamiliaMaxima> retornoApiMaxima = await apiMaxima.AlterarFamilias(listaAlterar);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoFamilia.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                var familiaModel = familiaDb.Where(c => c.Chave == item.CodigoFamilia.ToString()).FirstOrDefault();
                                if (familiaModel != null)
                                {
                                    familiaModel.Valor = item.Hash;
                                    dbContext.Update(familiaModel);
                                }

                            }
                            await dbContext.SaveChangesAsync();
                            log.AlterarOk(pagina, totalPaginas, listaAlterar.Count);
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

                var familiaList = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(x => x.Tabela == ControleDadosEnum.FAMILIAS)
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(familiaList)
                    .ToList();

                var familiaRemove = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave) && f.Tabela == ControleDadosEnum.FAMILIAS)
                    .ToListAsync();

                if (excluidos.Any())
                {
                    var listaExclusao = familiaRemove.Select(x => x.Valor);
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarFamilias(listaExclusao.ToArray());
                    if (retornoApiMaxima.Sucesso)
                    {
                        dbContext.ControleDadosModels.RemoveRange(familiaRemove);
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
