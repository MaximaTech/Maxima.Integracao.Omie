using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class MeiosDePagamentoOmie
    {
        public MeiosDePagamentoOmie()
        {
            contasCorrentes = new Dictionary<string, string>();
        }
        public string Key { get; set; }
        public string Value { get; set; }
        public object contasCorrentes { get; set; }
    }
}
