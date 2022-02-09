using System.Collections.Generic;
using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Requests
{
    public class RequestPedidoAPIOmie : RequestOmie
    {
        [JsonProperty("param")]
        public List<PedidoOmie> Pedidos { get; set; }
    }
}