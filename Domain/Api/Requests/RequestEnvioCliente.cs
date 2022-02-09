using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maxima.Cliente.Omie.Domain.Api.Requests
{
    public class RequestEnvioCliente
    {
        public RequestEnvioCliente()
        {
            param = new List<ClienteOmie>();
        }
        [JsonProperty("call")]
        public string Call { get; set; }

        [JsonProperty("app_key")]
        public string AppKey { get; set; } //= Configuracoes.ERP.Omie.AppKey;

        [JsonProperty("app_secret")]
        public string AppSecret { get; set; }
     
        [JsonProperty("param")]
        public List<ClienteOmie> param { get; set; }
    }
}
