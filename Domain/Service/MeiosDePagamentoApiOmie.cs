using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Data.Models;
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
    public class MeiosDePagamentoApiOmie : IMeiosDePagamentosApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;
        private readonly IContaCorrenteApiOmie _contaCorrenteApiOmie;


        public MeiosDePagamentoApiOmie(OmieContext context, IMapper mappe, MaximaIntegracao maximaIntegracao, IContaCorrenteApiOmie contaCorrenteApiOmie)
        {
            dbContext = context;
            mapper = mappe;
            this.apiMaxima = maximaIntegracao;
            _contaCorrenteApiOmie = contaCorrenteApiOmie;
        }

        public async Task EnviarMeiosPagamento(CancellationToken token)
        {
            LogApi log = new("Meios de Pagamentos");

            try
            {
                var meioPagamentoBd = await dbContext.ControleDadosModels
                    .Where(x => x.Tabela == ControleDadosEnum.MEIOPAGAMENTO)
                    .AsNoTracking()
                    .ToListAsync();

                var processados = new List<string>();
                var listaIncluir = new List<CobrancaMaxima>();
                var listaAlterar = new List<CobrancaMaxima>();

                var request = new RequestMeiosPagamento
                {
                    AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                    AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                    Call = "ListarMeiosPagamento"
                };

                var resultApi = await ApiUtilsMaxima.RequisicaoAsync<ResponseMeioPagamentoOmie, RequestMeiosPagamento>(MeiosPagamentoOmie.UrlApi, HttpMethod.Post, request);

                foreach (var pagamento in resultApi.Pagamentos)
                {
                    var mapCobranca = mapper.Map<CobrancaMaxima>(pagamento);

                    if (meioPagamentoBd.Any() && meioPagamentoBd.Any(x => x.Chave == mapCobranca.CodigoCobranca && x.Valor != mapCobranca.Hash))
                    {
                        listaAlterar.Add(mapCobranca);
                    }
                    else if (!meioPagamentoBd.Any(x => x.Chave == mapCobranca.CodigoCobranca))
                    {
                        listaIncluir.Add(mapCobranca);
                    }
                }

                if (listaIncluir.Any())
                {
                    ResponseApiMaxima<CobrancaMaxima> retornoApiMaxima = await apiMaxima.IncluirCobranca(listaIncluir);
                    if (retornoApiMaxima.Sucesso)
                    {
                        processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoCobranca.ToString()).ToList());
                        foreach (var item in retornoApiMaxima.ItensInserido)
                        {
                            ControleDadosModel meioPagamentoModel = new()
                            {
                                Tabela = ControleDadosEnum.MEIOPAGAMENTO,
                                Chave = item.CodigoCobranca,
                                Valor = item.Hash
                            };
                            dbContext.ControleDadosModels.Add(meioPagamentoModel);
                        }
                        dbContext.SaveChanges();
                        log.InserirOk(1, 1, retornoApiMaxima.ItensInserido.Count());
                        if (retornoApiMaxima.ErrosValidacao.Any())
                            log.InserirErro(1, 1, retornoApiMaxima.TotalItensNaoInserido, retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                    else
                    {
                        log.InserirErro(1, 1, listaIncluir.Count(), retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                }

                if (listaAlterar.Any())
                {
                    ResponseApiMaxima<CobrancaMaxima> retornoApiMaxima = await apiMaxima.AlterarCobranca(listaAlterar);
                    if (retornoApiMaxima.Sucesso)
                    {
                        processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoCobranca.ToString()).ToList());
                        foreach (var item in retornoApiMaxima.ItensInserido)
                        {
                            var meioPagamentoModel = meioPagamentoBd.Where(c => c.Chave == item.CodigoCobranca).FirstOrDefault();
                            if (meioPagamentoModel != null)
                            {
                                meioPagamentoModel.Valor = item.Hash;
                                dbContext.ControleDadosModels.Update(meioPagamentoModel);
                            }
                        }
                        dbContext.SaveChanges();
                        log.InserirOk(1, 1, retornoApiMaxima.ItensInserido.Count());

                        if (retornoApiMaxima.ErrosValidacao.Any())
                            log.InserirErro(1, 1, retornoApiMaxima.TotalItensNaoInserido, retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                    else
                    {
                        log.AlterarErro(1, 1, listaAlterar.Count, retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                }


                var meioPagamentoList = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(x => x.Tabela == ControleDadosEnum.MEIOPAGAMENTO)
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(meioPagamentoList)
                    .ToList();

                var meioPagamentoRemove = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave) && f.Tabela == ControleDadosEnum.MEIOPAGAMENTO)
                    .ToListAsync();

                if (excluidos.Any())
                    dbContext.ControleDadosModels.RemoveRange(meioPagamentoRemove);

                await dbContext.SaveChangesAsync();

                if (excluidos.Any())
                {
                    var listaExclusao = meioPagamentoRemove.Select(x => x.Valor);
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarFornecedor(listaExclusao.ToArray());
                    if (retornoApiMaxima.Sucesso)
                    {
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

        public async Task<List<MeiosDePagamentoOmie>> RecuperarMeiosdePagamentos()
        {
            try
            {
                var retorno = new List<MeiosDePagamentoOmie>();

                var request = new RequestMeiosPagamento
                {
                    AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                    AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                    Call = "ListarMeiosPagamento"
                };

                var resultApi = await ApiUtilsMaxima.RequisicaoAsync<ResponseMeioPagamentoOmie, RequestMeiosPagamento>(MeiosPagamentoOmie.UrlApi, HttpMethod.Post, request);

                foreach (var meioPagamento in resultApi.Pagamentos)
                {
                    MeiosDePagamentoOmie meiosPagamentos = new();
                    meiosPagamentos.Key = meioPagamento.codigo;
                    meiosPagamentos.Value = meioPagamento.descricao;
                    meiosPagamentos.contasCorrentes = _contaCorrenteApiOmie.RecuperarContaCorrente();
                    retorno.Add(meiosPagamentos);
                }

                return retorno;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
