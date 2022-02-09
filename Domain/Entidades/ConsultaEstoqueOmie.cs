namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class ConsultaEstoqueOmie
    {
        public string nCodProd { get; set; }                        // integer Código do Produto.+
        public string cCodInt { get; set; }                         // string20    Código de Integração do Produto.
        public string cCodigo { get; set; }                         // string60    Código do produto.+
        public string cDescricao { get; set; }                      // string120   Descrição do produto.
        public string nPrecoUnitario { get; set; }                  // decimal Preço Unitário de venda.
        public string nSaldo { get; set; }                          // decimal Saldo do produto.
        public string nCMC { get; set; }                            // decimal Custo Médio Contábil.
        public string nPendente { get; set; }                       // decimal Saldo pendente em pedidos de venda em aberto (não faturados) e não cancelados.
        public string estoque_minimo { get; set; }                  // decimal Estoque mínimo para o produto.
        public string codigo_local_estoque { get; set; }            // integer Código do Local do Estoque.


        public const string Endpoint = "estoque/consulta/";

        public const string VersaoAPI = "v1/";

        public const string UrlApi = "https://app.omie.com.br/api/v1/estoque/consulta/";

    }
}
