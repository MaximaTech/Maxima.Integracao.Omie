namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class FamiliaOmie
    {
        public string codigo { get; set; }                     // Código da Familia de Produto
        public string codInt { get; set; }                  // Código de Integração da Familia de Produto
        public string codFamilia { get; set; }              // Código da Familia
        public string nomeFamilia { get; set; }             // Nome da Familia de Produto
        public string inativo { get; set; }                 // Indica se a Familia de Produto está inativa[S / N]


        public const string VersaoAPI = "v1/";

        public const string Endpoint = "geral/familias/";

        public const string UrlApi = "https://app.omie.com.br/api/v1/geral/familias/";

    }
}
