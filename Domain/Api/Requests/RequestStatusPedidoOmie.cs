using System.Collections.Generic;
using Maxima.Cliente.Omie.Domain.Api.Parameters;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Requests
{
    public class RequestStatusPedidoOmie : RequestOmie
    {
        [JsonProperty("param")]
        public List<ParamStatusPedido> ParamStatusPedido { get; set; }
    }
}