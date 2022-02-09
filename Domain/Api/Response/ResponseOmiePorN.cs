using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseOmiePorN
    {
        [JsonProperty("nPagina")]
        public long Pagina { get; set; }

        [JsonProperty("nTotPaginas")]
        public long TotalDePaginas { get; set; }

        [JsonProperty("nRegistros")]
        public long Registros { get; set; }

        [JsonProperty("nTotRegistros")]
        public long TotalDeRegistros { get; set; }

        [JsonProperty("faultstring")]
        public string faultstring { get; set; }

        [JsonProperty("faultcode")]
        public string faultcode { get; set; }

      
    }
}
