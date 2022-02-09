namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class LocalEstoqueOmie
    {
        public long codigo_local_estoque { get; set; }           // Código do Local do Estoque.+
        public string codigo { get; set; }                      // Código do Local do Estoque.+
        public string descricao { get; set; }                   // Descrição do Local de Estoque.
        public string tipo { get; set; }                        // Tipo de Local de Estoque.+
        public string padrao { get; set; }                      // Indica se o Local do Estoque é padrão(S/N).
        public string inativo { get; set; }                     // Indica se o Local do Estoque está inativo(S/N).
        public long codigo_cliente { get; set; }                 // Código do cliente/fornecedor.
        public string dispOrdemProducao { get; set; }           // Indica se o Local do Estoque está disponível para Ordem de Produção(S/N).
        public string dispConsumoOP { get; set; }               // Indica se o Local do Estoque está disponível para consumo da Ordem de Produção(S/N).
        public string dispRemessa { get; set; }                 // Indica se o Local do Estoque está disponível para Remessa(S/N).
        public string dispVenda { get; set; }                   // Indica se o Local do Estoque está disponível para Venda(S/N).
        public string dInc { get; set; }                        // Data de Inclusão.
        public string hInc { get; set; }                        // Hora de inclusão.
        public string uInc { get; set; }                        // Código do usuário que realizou a inclusão.
        public string dAlt { get; set; }                        // Hora de alteração.
        public string hAlt { get; set; }                        // Hora de alteração.
        public string uAlt { get; set; }                        // Código do usuário que realizou a alteração.

        public const string VersaoAPI = "v1/estoque/";

        public const string Endpoint = "local/";
        public const string UrlApi = "https://app.omie.com.br/api/v1/estoque/local/";

    }
}
