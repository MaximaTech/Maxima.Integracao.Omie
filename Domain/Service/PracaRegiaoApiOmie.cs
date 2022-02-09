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
    public class PracaRegiaoApiOmie : IPracaRegiaoApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;

        public PracaRegiaoApiOmie(OmieContext context, IMapper mappe, MaximaIntegracao maximaIntegracao)
        {
            dbContext = context;
            mapper = mappe;
            this.apiMaxima = maximaIntegracao;
        }

        public async Task EnviarPracaRegiao(CancellationToken token)
        {
            LogApi log = new("Praça Região");

            long totalPaginas;
            var pagina = 1;
            do
            {
                var request = new RequestApiOmiePorN
                {
                    AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                    AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                    Call = "ListarTabelasPreco",
                    ParamsN = new List<ParamN> { new ParamN() { Pagina = pagina } }
                };
                var resultApi = await ApiUtilsMaxima.RequisicaoAsync<ResponseTabeladePrecoOmie, RequestApiOmiePorN>(TabelaPrecosOmie.UrlApi, HttpMethod.Post, request);

                totalPaginas = resultApi.TotalDePaginas;

                await ListarRegiao(token, resultApi.TabelasPreco);
                await ListarPraca(token, resultApi.TabelasPreco);

                pagina++;
            } while (pagina <= totalPaginas);

        }

        public async Task ListarPraca(CancellationToken token, List<TabelaPrecosOmie> pracas)
        {
            LogApi log = new("Praça Região");

            try
            {
                var pracaDb = await dbContext.ControleDadosModels
                    .Where(x => x.Tabela == ControleDadosEnum.PRACAS)
                    .AsNoTracking()
                    .ToListAsync();

                var processados = new List<string>();
                var listaIncluir = new List<PracaMaxima>();
                var listaAlterar = new List<PracaMaxima>();

                foreach (var praca in pracas)
                {

                    var mapPraca = mapper.Map<PracaMaxima>(praca);

                    if (pracaDb.Any() && pracaDb.Any(x => x.Chave == mapPraca.CodigoPraca && x.Valor != mapPraca.Hash))
                    {
                        listaAlterar.Add(mapPraca);
                    }
                    else if (!pracaDb.Any(x => x.Chave == mapPraca.CodigoPraca))
                    {
                        listaIncluir.Add(mapPraca);
                    }
                }

                if (listaIncluir.Any())
                {
                    ResponseApiMaxima<PracaMaxima> retornoApiMaxima = await apiMaxima.IncluirPracas(listaIncluir);
                    if (retornoApiMaxima.Sucesso)
                    {
                        processados.AddRange(pracas.Select(x => x.nCodTabPreco).ToList());
                        foreach (var item in retornoApiMaxima.ItensInserido)
                        {
                            ControleDadosModel pracaModel = new()
                            {
                                Tabela = ControleDadosEnum.PRACAS,
                                Chave = item.CodigoPraca,
                                Valor = item.Hash
                            };
                            dbContext.ControleDadosModels.Add(pracaModel);
                        }

                        await dbContext.SaveChangesAsync();
                        log.AlterarOk(1, 1, retornoApiMaxima.ItensInserido.Count());

                        if (retornoApiMaxima.ErrosValidacao.Any())
                            log.InserirErro(1, 1, retornoApiMaxima.TotalItensNaoInserido, retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                    else
                    {
                        log.InserirErro(1, 1, listaAlterar.Count(), retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                }

                if (listaAlterar.Any())
                {
                    ResponseApiMaxima<PracaMaxima> retornoApiMaxima = await apiMaxima.AlterarPracas(listaAlterar);
                    if (retornoApiMaxima.Sucesso)
                    {
                        processados.AddRange(pracas.Select(x => x.nCodTabPreco).ToList());
                        foreach (var item in retornoApiMaxima.ItensInserido)
                        {

                            var pracaModel = pracaDb.Where(c => c.Chave == item.CodigoPraca).FirstOrDefault();
                            if (pracaModel != null)
                            {
                                pracaModel.Valor = item.Hash;
                                dbContext.ControleDadosModels.Update(pracaModel);
                            }

                        }
                        await dbContext.SaveChangesAsync();
                        log.AlterarOk(1, 1, retornoApiMaxima.ItensInserido.Count());

                        if (retornoApiMaxima.ErrosValidacao.Any())
                            log.InserirErro(1, 1, retornoApiMaxima.TotalItensNaoInserido, retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                    else
                    {
                        dbContext.ChangeTracker.Clear();
                        log.AlterarErro(1, 1, listaAlterar.Count(), retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                }

                var pracaList = await dbContext.ControleDadosModels
                    .Where(x => x.Tabela == ControleDadosEnum.PRACAS)
                    .AsNoTracking()
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(pracaList)
                    .ToList();

                var pracaRemove = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave) && f.Tabela == ControleDadosEnum.PRACAS)
                    .ToListAsync();

                if (excluidos.Any())
                {
                    var listaExclusao = pracaRemove
                        .Select(x => x.Valor);

                    dbContext.ControleDadosModels.RemoveRange(pracaRemove);
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarPracas(listaExclusao.ToArray());
                    if (retornoApiMaxima.Sucesso)
                    {
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
            }
        }
        public async Task ListarRegiao(CancellationToken token, List<TabelaPrecosOmie> regioes)
        {
            LogApi log = new("Praça Região");
            try
            {
                var filiaisBd = dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.FILIAIS).AsNoTracking().FirstOrDefault();

                var regiaoDb = await dbContext.ControleDadosModels
                    .Where(x => x.Tabela == ControleDadosEnum.REGIAO)
                    .AsNoTracking()
                    .ToListAsync();

                var processados = new List<string>();
                var listaIncluir = new List<RegiaoMaxima>();
                var listaAlterar = new List<RegiaoMaxima>();

                foreach (var regiao in regioes)
                {

                    var mapRegiao = mapper.Map<RegiaoMaxima>(regiao);
                    mapRegiao.CodigoFilial = filiaisBd?.Chave;

                    if (regiaoDb.Any() && regiaoDb.Any(x => x.Chave == mapRegiao.CodigoRegiao && x.Valor != mapRegiao.Hash))
                    {
                        listaAlterar.Add(mapRegiao);
                    }
                    else if (!regiaoDb.Any(x => x.Chave == mapRegiao.CodigoRegiao))
                    {
                        listaIncluir.Add(mapRegiao);
                    }
                }

                if (listaIncluir.Any())
                {
                    ResponseApiMaxima<RegiaoMaxima> retornoApiMaxima = await apiMaxima.IncluirRegioes(listaIncluir);
                    if (retornoApiMaxima.Sucesso)
                    {
                        processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoRegiao).ToList());
                        foreach (var item in retornoApiMaxima.ItensInserido)
                        {
                            ControleDadosModel regiaoModel = new()
                            {
                                Tabela = ControleDadosEnum.REGIAO,
                                Chave = item.CodigoRegiao,
                                Valor = item.Hash
                            };

                            dbContext.ControleDadosModels.Add(regiaoModel);
                        }
                        await dbContext.SaveChangesAsync();
                        log.InserirOk(1, 1, retornoApiMaxima.ItensInserido.Count());

                        if (retornoApiMaxima.ErrosValidacao.Any())
                            log.InserirErro(1, 1, retornoApiMaxima.TotalItensNaoInserido, retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                    else
                    {
                        dbContext.ChangeTracker.Clear();
                        log.InserirErro(1, 1, listaAlterar.Count(), retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                }

                if (listaAlterar.Any())
                {
                    ResponseApiMaxima<RegiaoMaxima> retornoApiMaxima = await apiMaxima.AlterarRegioes(listaAlterar);
                    if (retornoApiMaxima.Sucesso)
                    {
                        processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoRegiao).ToList());
                        foreach (var item in retornoApiMaxima.ItensInserido)
                        {
                            var regiaoModel = regiaoDb.Where(c => c.Chave == item.CodigoRegiao).FirstOrDefault();
                            if (regiaoModel != null)
                            {
                                regiaoModel.Valor = item.Hash;
                                dbContext.Update(regiaoModel);
                            }
                        }
                        await dbContext.SaveChangesAsync();
                        log.InserirOk(1, 1, retornoApiMaxima.ItensInserido.Count());

                        if (retornoApiMaxima.ErrosValidacao.Any())
                            log.InserirErro(1, 1, retornoApiMaxima.TotalItensNaoInserido, retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                    else
                    {
                        dbContext.ChangeTracker.Clear();
                        log.AlterarErro(1, 1, listaAlterar.Count(), retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                }

                var regiaoList = await dbContext.ControleDadosModels
                    .Where(x => x.Tabela == ControleDadosEnum.REGIAO)
                    .AsNoTracking()
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(regiaoList)
                    .ToList();

                var regiaoRemove = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave) && f.Tabela == ControleDadosEnum.REGIAO)
                    .ToListAsync();

                if (excluidos.Any())
                {
                    var listaExclusao = regiaoRemove.Select(x => x.Valor);
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarRegioes(listaExclusao.ToArray());
                    if (retornoApiMaxima.Sucesso)
                    {
                        dbContext.ControleDadosModels.RemoveRange(regiaoRemove);
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
            }
        }


    }
}
