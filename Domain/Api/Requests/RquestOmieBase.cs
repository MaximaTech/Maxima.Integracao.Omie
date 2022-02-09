using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Requests
{
    public class RquestOmieBase
    {
        [JsonProperty("call")]
        public string Call { get; set; }

        [JsonProperty("app_key")]
        public string AppKey { get; set; }

        [JsonProperty("app_secret")]
        public string AppSecret { get; set; }

    }
}