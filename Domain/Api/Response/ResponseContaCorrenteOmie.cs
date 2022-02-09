using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseContaCorrenteOmie : ResponseOmie
    {
        public ResponseContaCorrenteOmie()
        {
            ContasCorrentes = new List<ContaCorrenteOmie>();
        }
        [JsonProperty("conta_corrente_lista")]
        public List<ContaCorrenteOmie> ContasCorrentes { get; set; }
    }
}
