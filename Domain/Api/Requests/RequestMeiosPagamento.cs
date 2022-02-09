using System.Collections.Generic;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Requests
{
    public class RequestMeiosPagamento : RquestOmieBase
    {
        public RequestMeiosPagamento()
        {
            Params = new List<ParamMeioPagamento>();
            Params.Add(new ParamMeioPagamento());
        }
        [JsonProperty("param")]
        public List<ParamMeioPagamento> Params { get; set; }
    }

    public class ParamMeioPagamento
    {
        public ParamMeioPagamento()
        {
            Codigo = "";
        }
        [JsonProperty("codigo")]
        public string Codigo { get; set; }
    }
}