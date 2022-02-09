using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseFamiliaOmie : ResponseOmie
    {
        public ResponseFamiliaOmie()
        {
            Familias = new List<FamiliaOmie>();
        }
        [JsonProperty("famCadastro")]
        public List<FamiliaOmie> Familias { get; set; }
    }
}
