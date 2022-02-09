using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Domain.Api.Response;
using Maxima.Cliente.Omie.Domain.Enums;
using Maxima.Cliente.Omie.Domain.Interfaces.Api;
using Maxima.Cliente.Omie.Domain.Utils;
using Maxima.Net.SDK.Integracao.Api;
using Maxima.Net.SDK.Integracao.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Maxima.Cliente.Omie.Domain.Service
{
    public class PedidoStatusApiOmie : IPedidoStatusApiOmie
    {
        private readonly IServiceProvider serviceProvider;
        private readonly MaximaIntegracao apiMaxima;
        private LogApi log;

        public PedidoStatusApiOmie(MaximaIntegracao apiMaxima, IServiceProvider serviceProvider)
        {
            this.apiMaxima = apiMaxima;
            this.serviceProvider = serviceProvider;
        }

        public async Task SincronizarStatusPedidosHistorico(CancellationToken token)
        {
            log = new("Status Pedido");
            try
            {
                using var dbContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<OmieContext>();

                var diaAnterior = DateTime.Now.AddDays(-5);
                var lisPedidos = dbContext.ControleDadosModels.Where(x => x.Tabela == ControleDadosEnum.PEDIDOS && x.Date.Date >= diaAnterior).AsNoTracking().ToList();

                foreach (var pedido in lisPedidos)
                {
                    ResponsePedidoOmie statusPedidoOmie;

                    statusPedidoOmie = await PedidoApiUtils.GetStatusPedidoOmie(idPedido: pedido.Chave, dbContext: dbContext);

                    if (statusPedidoOmie.Sucesso)
                    {

                        var criticaPedido = PedidoApiUtils.MontarDescricaoCritica(statusPedidoOmie);
                        var posicaoPedidoMaxima = PedidoApiUtils.RetornarPosicaoPedidoMaxima(statusPedidoOmie, dbContext);

                        RequestStatusPedido requestStatusPedido = new RequestStatusPedido();
                        requestStatusPedido.CodigoPedidoNuvemMaxima = pedido.Chave;
                        requestStatusPedido.NumeroPedidoRcaMaxima = pedido.Valor;
                        requestStatusPedido.NumeroPedidoERP = int.Parse(statusPedidoOmie.NumeroPedido);
                        requestStatusPedido.CriticaPedido = criticaPedido;
                        requestStatusPedido.PosicaoPedidoMaxima = posicaoPedidoMaxima;
                        RetornoApiMaxima retornoApiMaxima = await apiMaxima.AtualizarHistoricoPedido(requestStatusPedido);

                        if (retornoApiMaxima.Sucesso)
                        {
                            log.InserirPedidoOk(pedido.Chave);
                        }
                        else
                        {
                            log.InserirPedidoErro(pedido.Chave, retornoApiMaxima.Error);
                        }

                    }
                    else
                    {
                        log.InserirPedidoErro(pedido.Chave, statusPedidoOmie.faultstring);
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task StatusPedidosPubSub(ResponseStatusPedidoOmie responsePedidoOmie)
        {
            log = new("Status Pedido");
            try
            {
                using var dbContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<OmieContext>();

                if (string.IsNullOrEmpty(responsePedidoOmie.Event.CodIntPedido))
                    return;

                var statusPedidoOmie = new ResponsePedidoOmie
                {
                    NumeroPedido = responsePedidoOmie.Event.NumeroPedido,
                    Etapa = responsePedidoOmie.Event.Etapa,
                    DescricaoStatus = responsePedidoOmie.Event.EtapaDescr,
                    Cancelada = responsePedidoOmie.Event.Cancelada
                };

                if (responsePedidoOmie.Topic.Equals("VendaProduto.Excluida"))
                    statusPedidoOmie.Cancelada = "S";

                var pedidoModel = dbContext.ControleDadosModels.Where(p => p.Tabela == ControleDadosEnum.PEDIDOS && p.Chave == responsePedidoOmie.Event.CodIntPedido).FirstOrDefault();

                if (pedidoModel == null)
                    return;


                var criticaPedido = PedidoApiUtils.MontarDescricaoCritica(statusPedidoOmie);
                var posicaoPedidoMaxima = PedidoApiUtils.RetornarPosicaoPedidoMaxima(statusPedidoOmie, dbContext);

                RequestStatusPedido requestStatusPedido = new RequestStatusPedido();
                requestStatusPedido.CodigoPedidoNuvemMaxima = pedidoModel.Chave;
                requestStatusPedido.NumeroPedidoRcaMaxima = pedidoModel.Valor;
                requestStatusPedido.NumeroPedidoERP = int.Parse(statusPedidoOmie.NumeroPedido);
                requestStatusPedido.CriticaPedido = criticaPedido;
                requestStatusPedido.PosicaoPedidoMaxima = posicaoPedidoMaxima;
                RetornoApiMaxima retornoApiMaxima = await apiMaxima.AtualizarHistoricoPedido(requestStatusPedido);

                if (retornoApiMaxima.Sucesso)
                {
                    log.InserirPedidoOk(responsePedidoOmie.Event.NumeroPedido);
                }
                else
                {
                    log.InserirPedidoErro(responsePedidoOmie.Event.NumeroPedido, retornoApiMaxima.Error);
                }
            }
            catch (Exception e)
            {
                log.GlobalError(e.Message);
            }
        }
    }
}
