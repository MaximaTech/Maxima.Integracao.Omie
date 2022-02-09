using System;
using System.Collections.Generic;
using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Net.SDK.Integracao.Dto.Pedido;
using static Maxima.Cliente.Omie.Domain.Entidades.PedidoOmie;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll
    {

        public void PedidoMapping()
        {
            CreateMap<PedidoMaxima, PedidoOmie>()
                .BeforeMap((Maxima, omie) =>
                 {
                     omie.cabecalho = new Cabecalho();
                     omie.det = new List<Det>();
                     omie.informacoes_adicionais = new InformacoesAdicionais();
                     omie.frete = new PedidoOmie.Frete();
                 })
                 .ForPath(db => db.cabecalho.codigo_cliente, map => map.MapFrom(b => b.Cliente.Codigo))
                 .ForPath(db => db.cabecalho.codigo_pedido_integracao, map => map.MapFrom(b => b.CodigoPedidoNuvem))
                 .ForPath(db => db.cabecalho.data_previsao, map => map.MapFrom(b => b.DataPrevisaoFaturamento == null ? DateTime.Now.AddDays(2).ToString("dd/MM/yyyy") : b.DataPrevisaoFaturamento.Value.ToString("dd/MM/yyyy")))
                 .ForPath(db => db.cabecalho.etapa, map => map.MapFrom(b => "10"))
                 .ForPath(db => db.cabecalho.codigo_parcela, map => map.MapFrom(b => b.PlanoPagamento.Codigo))
                 .ForPath(db => db.observacoes.obs_venda, map => map.MapFrom(b => "Obs: " + b.Observacao + " Obs Entrega: " + b.ObservacaoEntrega))
                 .ForPath(db => db.informacoes_adicionais.enviar_email, map => map.MapFrom(b => "S"))
                 .ForPath(db => db.informacoes_adicionais.consumidor_final, map => map.MapFrom(b => "N"))
                 .ForPath(db => db.informacoes_adicionais.codVend, map => map.MapFrom(b => b.CodUsuario))
                 .ForPath(db => db.informacoes_adicionais.meio_pagamento, map => map.MapFrom(b => b.Cobranca.Codigo))
                 .AfterMap((Maxima, omie) =>
                 {
                     if (Maxima.TipoVenda.Codigo == 5)
                     {
                         Maxima.Produtos.ForEach(p => p.ItemBonificado = true);
                         omie.cabecalho.codigo_parcela = "000";
                         omie.informacoes_adicionais.meio_pagamento = "90";

                     }
                 });


            CreateMap<ProdutoPedido, Det>()
            .BeforeMap((Maxima, omie) =>
            {
                omie = new Det();
            })
            .ForPath(db => db.ide.codigo_item_integracao, map => map.MapFrom(b => b.Sequencia))
            .ForPath(db => db.produto.codigo_produto, map => map.MapFrom(b => b.Codigo))
            .ForPath(db => db.produto.unidade, map => map.MapFrom(b => b.Unidade))
            .ForPath(db => db.produto.ean, map => map.MapFrom(b => b.CodigoBarras))
            .ForPath(db => db.produto.quantidade, map => map.MapFrom(b => b.Quantidade))
            .ForPath(db => db.produto.valor_unitario, map => map.MapFrom(b => b.PrecoVenda))
            .ForPath(db => db.produto.percentual_desconto, map => map.MapFrom(b => b.PercDesconto))
            .ForPath(db => db.produto.tipo_desconto, map => map.MapFrom(b => "P"))
            .AfterMap((maxima, omie) =>
            {
                if (maxima.ItemBonificado)
                {
                    omie.produto.cfop = "5.910";
                    omie.inf_adic.dados_adicionais_item = "PRODUTO BONIFICADO";
                    omie.inf_adic.nao_gerar_financeiro = "S";
                }
            });


        }

    }
}