using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class EtapasFaturamentoOmie
    {
        public string cCodOperacao { get; set; }                 // Código da etapa do processo de faturamento.
        public string cDescOperacao { get; set; }              // Descrição da etapa do processo de faturamento.

        public List<Etapas> etapas { get; set; }

        public const string VersaoAPI = "v1/produtos/";

        public const string Endpoint = "etapafat/";
        public const string UrlApi = "https://app.omie.com.br/api/v1/produtos/etapafat/";

    }
    public class Etapas
    {
        public string cCodigo { get; set; }                 // Código da etapa do processo de faturamento.
        public string cDescricao { get; set; }              // Descrição da etapa do processo de faturamento.
        public string cDescrPadrao { get; set; }            // Descrição padrão da etapa do processo de faturamento.
    }
}
