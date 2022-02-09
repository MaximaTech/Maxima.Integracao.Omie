using System.Collections.Generic;
using Maxima.Cliente.Omie.Domain.Api.Parameters;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Requests
{
    public class RequestOmie : RquestOmieBase
    {
        public RequestOmie()
        {
            Params = new List<Param>();

        }

        [JsonProperty("param")]
        public List<Param> Params { get; set; }

    }
}