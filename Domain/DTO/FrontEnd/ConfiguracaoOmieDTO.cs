using System.Collections.Generic;

namespace Domain.DTO.FrontEnd
{
    public class ConfiguracaoOmieDTO
    {
        public string AppKey { get; set; }
        public string AppSecret { get; set; }
        public string EtapaFinalizador { get; set; }
        public string EtapaCargaInicialErro { get; set; }
        public string Erp { get; set; }
    }


    public class ConfiguracaoOmieEtapa2DTO
    {
        public string CodCategoria { get; set; }
        public string CodContaCorrente { get; set; }
        public string CodLocalEstoque { get; set; }
        public string ValorParcelaBoleto { get; set; }
        public string ValorFrete { get; set; }
        public List<string> CodEtapaOmie { get; set; }
        public List<string> CodEtapaMaxima { get; set; }
    }

    public class ConfiguracaoOmieEtapa3DTO
    {

        public List<string> ContaCorrente { get; set; }
        public List<string> MeiosDePagamento { get; set; }
    }
}
