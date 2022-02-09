using System;
using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Net.SDK.Integracao.Dto.Pedido;
using Maxima.Net.SDK.Integracao.Entidades;
using static Maxima.Cliente.Omie.Domain.Entidades.PedidoOmie;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll
    {

        public void HistoricoPedidoMapping()
        {
            CreateMap<PedidoOmie, HistoricoPedidoMaxima>()
                .BeforeMap((omie, maxima) =>
                 {
                     maxima = new HistoricoPedidoMaxima();
                 })
                  .ForPath(db => db.CapaPedido.NumeroPedido, map => map.MapFrom(b => int.Parse(b.cabecalho.numero_pedido)))
                  .ForPath(db => db.CapaPedido.CodigoPedidoNuvem, map => map.MapFrom(b => b.cabecalho.codigo_pedido_integracao))
                  .ForPath(db => db.CapaPedido.Data, map => map.MapFrom(b => DateTime.Parse(b.infoCadastro.dInc)))
                  .ForPath(db => db.CapaPedido.DataFaturamento, map => map.MapFrom<DateTime?>(b => b.infoCadastro.dFat != null ? DateTime.Parse(b.infoCadastro.dFat) : null))
                  .ForPath(db => db.CapaPedido.ValorTotal, map => map.MapFrom(b => b.total_pedido.valor_total_pedido))
                  .ForPath(db => db.CapaPedido.ValorTabela, map => map.MapFrom(b => b.total_pedido.valor_total_pedido))
                  .ForPath(db => db.CapaPedido.ValorAtendido, map => map.MapFrom(b => b.total_pedido.valor_total_pedido))
                  .ForPath(db => db.CapaPedido.ValorFrete, map => map.MapFrom(b => b.frete.valor_frete))
                  .ForPath(db => db.CapaPedido.CodigoCliente, map => map.MapFrom(b => b.cabecalho.codigo_cliente))
                  .ForPath(db => db.CapaPedido.CodigoVendedor, map => map.MapFrom(b => b.informacoes_adicionais.codVend))
                  .ForPath(db => db.CapaPedido.CodigoFilial, map => map.MapFrom(b => b.cabecalho.codigo_empresa))
                  .ForPath(db => db.CapaPedido.TipoVenda, map => map.MapFrom(b => 1))
                  .ForPath(db => db.CapaPedido.NumeroTransacao, map => map.MapFrom(b => b.cabecalho.numero_pedido))
                  .ForPath(db => db.CapaPedido.CodigoPlanoDePagamento, map => map.MapFrom(b => b.cabecalho.codigo_parcela))
                  .ForPath(db => db.CapaPedido.OrigemPedido, map => map.MapFrom(b => b.cabecalho.origem_pedido.Equals("API") ? "F" : "T"))
                  .ForPath(db => db.CapaPedido.Observacao, map => map.MapFrom(b => b.observacoes.obs_venda))
                  ;

            CreateMap<Det, HistoricoPedidoItem>()
            .BeforeMap((omie, maxima) =>
            {
                omie = new Det();
            })
            .ForPath(db => db.CodigoProduto, map => map.MapFrom(b => b.produto.codigo_produto))
            .ForPath(db => db.CodigoDeBarras, map => map.MapFrom(b => b.produto.ean))
            .ForPath(db => db.Quantidade, map => map.MapFrom(b => b.produto.quantidade))
            .ForPath(db => db.PrecoVenda, map => map.MapFrom(b => b.produto.valor_unitario))
            .ForPath(db => db.PrecoTabela, map => map.MapFrom(b => b.produto.valor_unitario))
            ;


        }

    }
}