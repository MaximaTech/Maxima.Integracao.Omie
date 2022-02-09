using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseFilialOmie : ResponseOmie
    {
        public ResponseFilialOmie()
        {
            Filiais = new List<FilialOmie>();
        }
        [JsonProperty("empresas_cadastro")]
        public List<FilialOmie> Filiais { get; set; }
    }
}
