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
    public class BancoApiOmie : IBancoApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;

        public BancoApiOmie(OmieContext context, IMapper mappe, MaximaIntegracao maximaIntegracao)
        {
            dbContext = context;
            mapper = mappe;
            this.apiMaxima = maximaIntegracao;
        }

        public async Task EnviarBancos(CancellationToken token)
        {
            LogApi log = new("Bancos");

            try
            {
                var bancosDb = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.BANCOS).AsNoTracking().ToListAsync();
                var pagina = 1;
                long totalPaginas;
                var processados = new List<string>();
                long countOmie;
                do
                {
                    var listaIncluir = new List<BancoMaxima>();
                    var listaAlterar = new List<BancoMaxima>();

                    var request = new RequestOmie
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "ListarBancos",
                        Params = new List<Param>() { new Param() { Pagina = pagina } }
                    };

                    var resultApi = await ApiUtilsMaxima.RequisicaoAsync<ResponseBancoOmie, RequestOmie>(BancoOmie.UrlApi, HttpMethod.Post, request);

                    totalPaginas = resultApi.TotalDePaginas;
                    countOmie = resultApi.TotalDeRegistros;

                    foreach (var bancoOmie in resultApi.Bancos)
                    {
                        var produtoMap = mapper.Map<BancoMaxima>(bancoOmie);

                        if (bancosDb.Any(x => x.Chave == produtoMap.CodigoBanco && x.Valor != produtoMap.Hash))
                        {
                            listaAlterar.Add(produtoMap);
                        }
                        else if (!bancosDb.Any(x => x.Chave == produtoMap.CodigoBanco))
                        {
                            listaIncluir.Add(produtoMap);
                        }
                    }

                    if (listaIncluir.Any())
                    {
                        ResponseApiMaxima<BancoMaxima> retornoApiMaxima = await apiMaxima.IncluirBancos(listaIncluir);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoBanco.ToString()).ToList());

                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                ControleDadosModel produtoModel = new()
                                {
                                    Tabela = ControleDadosEnum.BANCOS,
                                    Chave = item.CodigoBanco,
                                    Valor = item.Hash
                                };
                                dbContext.ControleDadosModels.Add(produtoModel);

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
                        ResponseApiMaxima<BancoMaxima> retornoApiMaxima = await apiMaxima.AlterarBancos(listaAlterar);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoBanco.ToString()).ToList());

                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                var bancoDb = bancosDb.Where(c => c.Chave == item.CodigoBanco).FirstOrDefault();
                                if (bancosDb != null)
                                {
                                    bancoDb.Valor = item.Hash;
                                    dbContext.Update(bancoDb);
                                }

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

                    if (!listaAlterar.Any() && !listaIncluir.Any())
                        log.NenhumRegistroAlterado(pagina, totalPaginas);

                    pagina++;
                } while (pagina <= totalPaginas);

                var bancoList = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(w => w.Tabela == ControleDadosEnum.BANCOS)
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(bancoList)
                    .ToList();

                var bancoRemove = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave) && f.Tabela == ControleDadosEnum.BANCOS)
                    .ToListAsync();

                if (excluidos.Any())
                {
                    var listaExclusao = bancoRemove.Select(x => x.Valor);
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarBancos(listaExclusao.ToArray());
                    if (retornoApiMaxima.Sucesso)
                    {
                        dbContext.ControleDadosModels.RemoveRange(bancoRemove);
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
