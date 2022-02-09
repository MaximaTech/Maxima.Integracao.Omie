namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class VendedorOmie
    {
        public long codigo { get; set; }                             // Código do Vendedor
        public string codInt { get; set; }                          // Código de Integração do Vendedor
        public string nome { get; set; }                            // Nome do Vendedor
        public string inativo { get; set; }                         // Indica se o vendedor está inativo[S / N]
        public string email { get; set; }                           // E-mail do vendedor.
        public string fatura_pedido { get; set; }                   // O vendedor pode faturar um pedido?+
        public string visualiza_pedido { get; set; }                // Visualiza apenas os pedidos em que é o vendedor.+
        public decimal comissao { get; set; }                       // Percentual de Comissão.

        public const string UrlApi = "https://app.omie.com.br/api/v1/geral/vendedores/";
        public const string VersaoAPI = "v1/geral/";
        public const string Endpoint = "vendedores/";
    }
}
