namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class MeiosPagamentoOmie
    {
        public string codigo { get; set; }                  // Código do meio de pagamento.
        public string descricao { get; set; }               // Descrição do meio de pagamento.


        public const string Endpoint = "geral/meiospagamento/";
        public const string VersaoAPI = "v1/";
        public const string UrlApi = "https://app.omie.com.br/api/v1/geral/meiospagamento/";

    }
}
