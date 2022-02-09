using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class EtapaPedidoOmie
    {
        public EtapaPedidoOmie()
        {
            EtapasMaxima = new Dictionary<string, string>();
        }
        public string Key { get; set; }
        public string Value { get; set; }
        public object EtapasMaxima { get; set; }
    }
}
