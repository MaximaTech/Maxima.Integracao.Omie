using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseEtapaPedidoOmie : ResponseOmie
    {
        public ResponseEtapaPedidoOmie()
        {
            EtapasOmie = new List<EtapasFaturamentoOmie>();
        }
        [JsonProperty("cadastros")]
        public List<EtapasFaturamentoOmie> EtapasOmie { get; set; }

    }
}
