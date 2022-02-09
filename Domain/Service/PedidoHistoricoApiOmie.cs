using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using System.Threading;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Data.Models;
using Maxima.Cliente.Omie.Domain.Api.Response;
using Maxima.Cliente.Omie.Domain.Api.Requests;
using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Cliente.Omie.Domain.Enums;
using Maxima.Cliente.Omie.Domain.Interfaces.Api;
using Maxima.Cliente.Omie.Domain.Utils;
using Maxima.Net.SDK.Integracao.Api;
using Maxima.Net.SDK.Integracao.Dto;
using Maxima.Net.SDK.Integracao.Dto.Pedido;
using Maxima.Net.SDK.Integracao.Entidades;
using Maxima.Net.SDK.Integracao.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Maxima.Cliente.Omie.Domain.Service
{
    public class PedidoHistoricoApiOmie : IPedidoHistoricoApiOmie
    {
        private readonly IServiceProvider serviceProvider;
        private readonly MaximaIntegracao apiMaxima;
        private readonly IMapper mapper;

        private readonly LogApi log = new("Historio de pedidos");
        public PedidoHistoricoApiOmie(MaximaIntegracao apiMaxima, IMapper mapper, IServiceProvider serviceProvider)
        {
            this.apiMaxima = apiMaxima;
            this.mapper = mapper;
            this.serviceProvider = serviceProvider;
        }

        public async Task HistoricoPedidos(CancellationToken token)
        {
            try
            {
                using var ctx = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<OmieContext>();

                var pagina = 1;
                long totalPaginas;
                DateTime dateLimiteStatus = DateTime.Now.AddDays(-5);

                var meioPagamento = ctx.ControleDadosModels
                    .Where(e => e.Tabela == ControleDadosEnum.MEIOPAGAMENTO)
                    .FirstOrDefault().Chave;

                do
                {
                    var pedidosHistoricoList = new List<HistoricoPedidoMaxima>();

                    ResponseHistoricoPedidoOmie result = await RequisicaoHistoricoPedidoOmie(pagina);
                    totalPaginas = result.TotalDePaginas;

                    foreach (var pedidoOmie in result.Pedidos)
                    {
                        var historicoPedido = mapper.Map<HistoricoPedidoMaxima>(pedidoOmie);
                        historicoPedido.ItensPedido = mapper.Map<List<PedidoOmie.Det>, List<HistoricoPedidoItem>>(pedidoOmie.det);
                        historicoPedido.CapaPedido.PosicaoPedido = Utils.PedidoApiUtils.RetornarPosicaoHistoricoPedido(pedidoOmie, ctx);
                        historicoPedido.CapaPedido.CodigoCobranca = meioPagamento;
                        pedidosHistoricoList.Add(historicoPedido);

                    }

                    ResponseApiMaxima<HistoricoPedidoMaxima> retornoApiMaxima = await apiMaxima.IncluirHistoricoPedido(pedidosHistoricoList);

                    if (retornoApiMaxima.Sucesso)
                    {
                        foreach (var item in retornoApiMaxima.ItensInserido)
                        {
                            await AdcionarAoControleLocal(ctx, dateLimiteStatus, item);
                        }

                        log.InserirOk(pagina, totalPaginas, retornoApiMaxima.ItensInserido.Count());

                        if (retornoApiMaxima.ErrosValidacao.Any())
                            log.InserirErro(pagina, totalPaginas, retornoApiMaxima.TotalItensNaoInserido, retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                    else
                    {
                        log.InserirErro(pagina, totalPaginas, pedidosHistoricoList.Count(), retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                    pagina++;

                } while (pagina <= totalPaginas);


            }
            catch (Exception ex)
            {
                log.GlobalError(ex.Message);
                throw;
            }
        }

        private static async Task AdcionarAoControleLocal(OmieContext ctx, DateTime dateLimiteStatus, HistoricoPedidoMaxima historicoPedido)
        {
            if (historicoPedido.CapaPedido.Data.Value.CompareTo(dateLimiteStatus) >= 0 && !string.IsNullOrEmpty(historicoPedido.CapaPedido.CodigoPedidoNuvem))
            {
                if (!ctx.ControleDadosModels.Any(p => p.Tabela == ControleDadosEnum.PEDIDOS && p.Chave == historicoPedido.CapaPedido.CodigoPedidoNuvem))
                {
                    ControleDadosModel pedidoModel = new ControleDadosModel()
                    {
                        Tabela = ControleDadosEnum.PEDIDOS,
                        Chave = historicoPedido.CapaPedido.CodigoPedidoNuvem,
                        Valor = historicoPedido.CapaPedido.CodigoPedidoNuvem,
                        Date = historicoPedido.CapaPedido.Data.Value
                    };
                    ctx.ControleDadosModels.Add(pedidoModel);
                    await ctx.SaveChangesAsync();
                }
            }
        }

        private async Task<ResponseHistoricoPedidoOmie> RequisicaoHistoricoPedidoOmie(int pagina)
        {
            var dbContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<OmieContext>();

            var req = new RequestHistoricoPedidoOmie
            {
                AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                Call = "ListarPedidos",
                ParamStatusPedido = new List<ParamHistoricoPedido>(){

                    new ParamHistoricoPedido(){
                        Pagina = pagina,
                        ApenasImportadoApi = "N",
                        FiltrarApenasInclusao = "S",
                        DataEntradaDe = DateTime.Now.AddDays(-60).ToString("dd/MM/yyyy"),
                        DataEntradaAte = DateTime.Now.ToString("dd/MM/yyyy")
                    }
                }
            };

            return await ApiUtilsMaxima.RequisicaoAsync<ResponseHistoricoPedidoOmie, RequestHistoricoPedidoOmie>(ConstantesEnum.UrlBaseOmie + PedidoOmie.VersaoAPI + PedidoOmie.Endpoint, HttpMethod.Post, req);
        }
    }
}