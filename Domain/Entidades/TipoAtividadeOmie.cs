namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class TipoAtividadeOmie
    {
        public string cCodigo { get; set; } //Código do Tipo de Atividade da Empresa.

        public string cDescricao { get; set; } //Descrição do Tipo de Atividade da Empresa.


        public const string UrlApi = "https://app.omie.com.br/api/v1/geral/tpativ/";
        public const string VersaoAPI = "v1/geral/";
        public const string Endpoint = "tpativ/";
    }
}
