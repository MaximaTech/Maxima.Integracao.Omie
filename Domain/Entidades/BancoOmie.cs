using System.ComponentModel.DataAnnotations;

namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class BancoOmie
    {
        [MaxLength(3)]
        public string codigo { get; set; } //  Código do Banco / Instituição Financeira

        [MaxLength(40)]
        public string nome { get; set; } //  Nome do Banco

        [MaxLength(2)]
        public string tipo { get; set; }//  Tipo da Conta Corrente (CB Conta Bancária, CX Caixinha, CC Cartão de Crédito ou CV Carteira Virtual)


        public const string VersaoAPI = "v1/geral/";

        public const string Endpoint = "bancos/";

        public const string UrlApi = "https://app.omie.com.br/api/v1/geral/bancos/";
    }
}