using System.Collections.Generic;
using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseClienteOmie : ResponseOmie
    {
        public ResponseClienteOmie()
        {
            Clientes = new List<ClienteOmie>();
        }
        
        [JsonProperty("clientes_cadastro")]
        public List<ClienteOmie> Clientes { get; set; }
    }
}