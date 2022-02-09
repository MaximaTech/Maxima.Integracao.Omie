using Maxima.Cliente.Omie.Domain.Api.Parameters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Api.Requests
{
    public class RequestTipoAtividadeOmie
    {
        [JsonProperty("call")]
        public string Call { get; set; }

        [JsonProperty("app_key")]
        public string AppKey { get; set; }

        [JsonProperty("app_secret")]
        public string AppSecret { get; set; }
        [JsonProperty("param")]
        public List<ParamTipoAtividade> Params { get; set; }
    }
}
