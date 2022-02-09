using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseCidadesOmie : ResponseOmie
    {
        public ResponseCidadesOmie()
        {
            Cidades = new List<CidadeOmie>();
        }
        [JsonProperty("lista_cidades")]
        public List<CidadeOmie> Cidades { get; set; }
    }
}
