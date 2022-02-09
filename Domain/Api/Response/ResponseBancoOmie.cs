using System.Collections.Generic;
using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseBancoOmie : ResponseOmie
    {
        public ResponseBancoOmie()
        {
            Bancos = new List<BancoOmie>();
        }
        [JsonProperty("fin_banco_cadastro")]
        public List<BancoOmie> Bancos { get; set; }

    }
}