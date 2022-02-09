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
using Maxima.Net.SDK.Integracao.Dto.Pedido;
using Maxima.Net.SDK.Integracao.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static Maxima.Cliente.Omie.Domain.Entidades.PedidoOmie;

namespace Maxima.Cliente.Omie.Domain.Service
{
    public class PedidoEnvioApiOmie : IPedidoEnvioApiOmie
    {

        private readonly IServiceProvider serviceProvider;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;
        public PedidoEnvioApiOmie(IMapper mapper, MaximaIntegracao maximaIntegracao, IServiceProvider serviceProvider)
        {
            this.mapper = mapper;
            this.apiMaxima = maximaIntegracao;
            this.serviceProvider = serviceProvider;
        }
        public async Task ReceberPedidos(CancellationToken token)
        {
            var pedidosMaxima = await apiMaxima.GetPedidoNaoImportados();

            foreach (var pedido in pedidosMaxima)
            {
                await EnviarPedidoAsync(pedido);
            }

        }
        public async Task EnviarPedidoAsync(PedidoMaxima pedidoMaxima)
        {
            LogApi log = new LogApi("Pedido Envio Numero: " + pedidoMaxima.NumPedido);
            try
            {
                using var dbContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<OmieContext>();

                RetornoApiMaxima retornoApiMaxima = null;

                await InserirPedidoNoControleLocal(pedidoMaxima, dbContext);
                var pedidoConvertido = ConverterPedidoMaximaParaOmie(pedidoMaxima, dbContext);
                var retornoEnvioPedidoOmie = await EnviarPedidoParaOmie(pedidoConvertido);

                if (retornoEnvioPedidoOmie.Sucesso)
                {
                    var criticaPedido = PedidoApiUtils.MontarDescricaoCritica(retornoEnvioPedidoOmie);
                    var posicaoPedidoMaxima = PedidoApiUtils.RetornarPosicaoPedidoMaxima(retornoEnvioPedidoOmie, dbContext);
                    retornoApiMaxima = await apiMaxima.IncluirHistoricoPedidoImportado(pedidoMaxima, int.Parse(retornoEnvioPedidoOmie.NumeroPedido), criticaPedido, posicaoPedidoMaxima);

                    if (retornoApiMaxima.Sucesso)
                    {
                        log.InserirPedidoOk(pedidoMaxima.NumPedido);
                    }
                    else
                    {
                        log.InserirPedidoErro(pedidoMaxima.NumPedido, retornoApiMaxima.Error);
                    }
                }
                else
                {
                    if (retornoEnvioPedidoOmie.PedidoJaIncluidoAnteriormente)
                    {
                        ResponsePedidoOmie statusPedidoOmie = await PedidoApiUtils.GetStatusPedidoOmie(pedidoMaxima.CodigoPedidoNuvem, dbContext);
                        if (statusPedidoOmie.Sucesso)
                        {
                            var criticaPedido = PedidoApiUtils.MontarDescricaoCritica(statusPedidoOmie);
                            var posicaoPedidoMaxima = PedidoApiUtils.RetornarPosicaoPedidoMaxima(statusPedidoOmie, dbContext);
                            RequestStatusPedido requestStatusPedido = new RequestStatusPedido();
                            requestStatusPedido.CodigoPedidoNuvemMaxima = pedidoMaxima.CodigoPedidoNuvem;
                            requestStatusPedido.NumeroPedidoRcaMaxima = pedidoMaxima.NumPedido;
                            requestStatusPedido.NumeroPedidoERP = int.Parse(statusPedidoOmie.NumeroPedido);
                            requestStatusPedido.CriticaPedido = criticaPedido;
                            requestStatusPedido.PosicaoPedidoMaxima = posicaoPedidoMaxima;

                            retornoApiMaxima = await apiMaxima.AtualizarHistoricoPedido(requestStatusPedido);

                            if (retornoApiMaxima.Sucesso)
                            {
                                log.InserirPedidoOk(pedidoMaxima.NumPedido);
                            }
                            else
                            {
                                log.InserirPedidoErro(pedidoMaxima.NumPedido, retornoApiMaxima.Error);
                            }
                        }
                        else
                        {
                            log.GlobalError($"NumPedido:{pedidoMaxima.NumPedido} Não foi possivel consultar o status do pedido no Omie");
                        }
                    }
                    else
                    {

                        RequestStatusPedido requestStatusPedido = new RequestStatusPedido();
                        requestStatusPedido.CodigoPedidoNuvemMaxima = pedidoMaxima.CodigoPedidoNuvem;
                        requestStatusPedido.NumeroPedidoRcaMaxima = pedidoMaxima.NumPedido;
                        requestStatusPedido.CriticaPedido = retornoEnvioPedidoOmie.faultstring;
                        requestStatusPedido.PosicaoPedidoMaxima = EnumPosicaoPedido.Desconhecido;
                        retornoApiMaxima = await apiMaxima.AtualizarStatusPedido(requestStatusPedido, true);

                        log.GlobalError($"NumPedido:{pedidoMaxima.NumPedido} {retornoEnvioPedidoOmie.faultstring}");
                    }
                }

                if (retornoApiMaxima != null && !retornoApiMaxima.Sucesso)
                {
                    log.GlobalError($"NumPedido:{pedidoMaxima.NumPedido} {retornoApiMaxima.Error}");
                }

            }
            catch (Exception ex)
            {
                log.GlobalError(ex.Message);
            }
        }
        private async Task<ResponsePedidoOmie> EnviarPedidoParaOmie(PedidoOmie mapPedidoOmie)
        {
            var dbContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<OmieContext>();

            RequestPedidoAPIOmie request = new RequestPedidoAPIOmie
            {
                AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                Call = "IncluirPedido",
                Pedidos = new List<PedidoOmie>() { mapPedidoOmie }
            };
            ResponsePedidoOmie retornoEnvioPedidoOmie = await ApiUtilsMaxima.RequisicaoAsync<ResponsePedidoOmie, RequestPedidoAPIOmie>(ConstantesEnum.UrlBaseOmie + PedidoOmie.VersaoAPI + PedidoOmie.Endpoint, HttpMethod.Post, request);
            return retornoEnvioPedidoOmie;
        }
        private PedidoOmie ConverterPedidoMaximaParaOmie(PedidoMaxima pedidoMaxima, OmieContext dbContext)
        {
            var codigoCategoria = dbContext.Parametros.AsNoTracking().Where(x => x.Nome.Contains(ConstantesEnum.CategoriaOmie)).FirstOrDefault();
            var contacorrenteModel = dbContext.ControleDadosModels.AsNoTracking().Where(c => c.Tabela == ControleDadosEnum.MEIOPAGAMENTOCOMPARACAO && c.Chave == pedidoMaxima.Cobranca.Codigo).FirstOrDefault();
            var contacorrente = contacorrenteModel != null ? contacorrenteModel.Valor : dbContext.ControleDadosModels.AsNoTracking().Where(c => c.Tabela == ControleDadosEnum.MEIOPAGAMENTOCOMPARACAO).First().Valor;
            var valorFrete = dbContext.Parametros.Where(c => c.Nome == ConstantesEnum.ValorFreteOmie).FirstOrDefault();
            var valorParcelaBoleto = dbContext.Parametros.Where(c => c.Nome == ConstantesEnum.ValorParcelaBoletoOmie).FirstOrDefault();

            PedidoOmie mapPedidoOmie = mapper.Map<PedidoOmie>(pedidoMaxima);
            mapPedidoOmie.frete = new PedidoOmie.Frete();
            mapPedidoOmie.informacoes_adicionais.codigo_categoria = codigoCategoria.Valor;
            mapPedidoOmie.informacoes_adicionais.codigo_conta_corrente = contacorrente;

            if (!string.IsNullOrEmpty(valorFrete.Valor) && double.Parse(valorFrete.Valor) > 0)
            {
                mapPedidoOmie.frete.valor_frete = Convert.ToDecimal(valorFrete.Valor);
            }
            if (!string.IsNullOrEmpty(valorParcelaBoleto.Valor) && double.Parse(valorParcelaBoleto.Valor) > 0)
            {
                mapPedidoOmie.frete.outras_despesas = (pedidoMaxima.PlanoPagamento?.NumeroParcelas ?? 0) * decimal.Parse(valorParcelaBoleto.Valor);
            }

            mapPedidoOmie.det = mapper.Map<List<ProdutoPedido>, List<Det>>(pedidoMaxima.Produtos);
            return mapPedidoOmie;
        }
        private static async Task InserirPedidoNoControleLocal(PedidoMaxima pedidoMaxima, OmieContext dbContext)
        {
            if (!dbContext.ControleDadosModels.Any(p => p.Tabela == ControleDadosEnum.PEDIDOS && p.Chave == pedidoMaxima.CodigoPedidoNuvem))
            {
                ControleDadosModel pedidoModel = new ControleDadosModel()
                {
                    Tabela = ControleDadosEnum.PEDIDOS,
                    Chave = pedidoMaxima.CodigoPedidoNuvem,
                    Valor = pedidoMaxima.NumPedido,
                    Date = DateTime.Now
                };
                dbContext.ControleDadosModels.Add(pedidoModel);
                await dbContext.SaveChangesAsync();
            }
        }



    }
}
