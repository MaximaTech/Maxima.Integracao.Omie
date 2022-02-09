namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class DadosDREOmie
    {
        public string codigoDRE { get; set; } // Código da Conta do DRE.
        public string descricaoDRE { get; set; } // Descrição da Conta do DRE.
        public string naoExibirDRE { get; set; } // Indica se a Conta está marcada para não exibir no DRE.
        public int nivelDRE { get; set; } // Nível da Conta da DRE.
        public string sinalDRE { get; set; } // Sinal da Conta para o DRE.
        public string totalizaDRE { get; set; } // Indica se a Conta do DRE é Totalizadora.


        public const string VersaoAPI = "v1/geral/";

        public const string Endpoint = "dre/";
    }
}
