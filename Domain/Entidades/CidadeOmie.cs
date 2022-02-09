namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class CidadeOmie
    {
        public string cCod { get; set; } //  Código da Cidade
        public string cNome { get; set; } //  Nome da Cidade
        public string cUF { get; set; } //  UF da cidade
        public string nCodIBGE { get; set; } //	 Código IBGE da Cidade
        public int nCodSIAFI { get; set; }  //  Código SIAFI da Cidade


        public const string VersaoAPI = "v1/geral/";

        public const string Endpoint = "cidades/";
        public const string UrlApi = "https://app.omie.com.br/api/v1/geral/cidades/";

    }
}
