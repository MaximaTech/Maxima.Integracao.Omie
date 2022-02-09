namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class FormaPagamentoOmie
    {
        public string cCodigo { get; set; }             // Código da forma de pagamento.
        public int nQtdeParc { get; set; }              // Quantidade de parcelas.
        public string cDescricao { get; set; }          // Descrição da forma de pagamento.
        public string cListaParc { get; set; }          // Lista de dias de vencimentos.
        public int nDiasParc { get; set; }              // Dias de deslocamento para geração da parcela.

        public const string VersaoAPI = "v1/";
        public const string Endpoint = "produtos/formaspagvendas/";
        public const string UrlApi = "https://app.omie.com.br/api/v1/produtos/formaspagvendas/";

    }
}
