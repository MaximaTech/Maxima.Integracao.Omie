namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class DepartamentoOmie
    {
        public string codigo { get; set; }             // Código do Departamento / Centro de Custo
        public string descricao { get; set; }          // Nome do Departamento / Centro de Custo
        public string estrutura { get; set; }          // Utilizado para estruturação dos Departamentos no modo "diagrama"
        public string inativo { get; set; }            // Indica que o Departamento / Centro de Custo está inativo
        public string nivel_totalizador { get; set; }  // Indica que o Departamento / Centro de Custo é um nível totalizador

        public const string UrlApi = "https://app.omie.com.br/api/v1/geral/departamentos/";
        public const string VersaoAPI = "v1/geral/";
        public const string Endpoint = "departamentos/";
    }
}
