using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class JobsCronOmie
    {
        public static Dictionary<string, string> GetJobsCron()
        {
            Dictionary<string, string> jobs = new Dictionary<string, string>();

            jobs.Add(JobsCronEnumOmie.EnviarClientes.ToString(), "Enviar Clientes");
            jobs.Add(JobsCronEnumOmie.EnviarFiliais.ToString(), "Enviar Filiais");
            jobs.Add(JobsCronEnumOmie.EnviarFornecedores.ToString(), "Enviar Fornecedores");
            jobs.Add(JobsCronEnumOmie.EnviarTransportadora.ToString(), "Enviar Transportadora");
            jobs.Add(JobsCronEnumOmie.EnviarCidade.ToString(), "Enviar Cidades");
            jobs.Add(JobsCronEnumOmie.EnviarTipoAtividade.ToString(), "Enviar Tipo Atividades");
            jobs.Add(JobsCronEnumOmie.EnviarEstoque.ToString(), "Enviar Estoques");
            jobs.Add(JobsCronEnumOmie.EnviarProdutosPorFornecedor.ToString(), "Enviar Produtos Por Fornecedores");
            jobs.Add(JobsCronEnumOmie.EnviarBancos.ToString(), "Enviar Bancos");
            jobs.Add(JobsCronEnumOmie.EnviarVendedores.ToString(), "Enviar Vendedores");
            jobs.Add(JobsCronEnumOmie.EnviarTabelaPreco.ToString(), "Enviar Tabela de Precços");
            jobs.Add(JobsCronEnumOmie.EnviarProdutos.ToString(), "Enviar Produtos");
            jobs.Add(JobsCronEnumOmie.EnviarPracaRegiao.ToString(), "Enviar Praça/Região");
            jobs.Add(JobsCronEnumOmie.EnviarMeiosPagamento.ToString(), "Enviar Meios de Pagamento");
            jobs.Add(JobsCronEnumOmie.EnviarPrecoProdutos.ToString(), "Enviar Preço de Produtos");
            jobs.Add(JobsCronEnumOmie.EnviarTitulos.ToString(), "Enviar Titulos em Aberto");
            jobs.Add(JobsCronEnumOmie.EnviarFormaDePagamento.ToString(), "Enviar Formas de Pagamento");
            jobs.Add(JobsCronEnumOmie.SincronizarPedidosNuvemAsync.ToString(), "Sincronizar Pedidos");
            jobs.Add(JobsCronEnumOmie.SincronizarStatusPedido.ToString(), "Sincronizar Status de Pedido");
            jobs.Add(JobsCronEnumOmie.SincronizarClientes.ToString(), "Sincronizar cliente");

            return jobs;
        }
    }

    public enum JobsCronEnumOmie
    {

        EnviarEstoque,
        EnviarProdutosPorFornecedor,
        EnviarFornecedores,
        EnviarPracaRegiao,
        EnviarFiliais,
        EnviarCidade,
        EnviarTransportadora,
        EnviarClientes,
        EnviarTipoAtividade,
        EnviarBancos,
        SincronizarPedidosNuvemAsync,
        EnviarVendedores,
        EnviarTabelaPreco,
        EnviarProdutos,
        EnviarMeiosPagamento,
        EnviarFormaDePagamento,
        SincronizarStatusPedido,
        EnviarPrecoProdutos,
        EnviarTitulos,
        SincronizarClientes

    }
}

