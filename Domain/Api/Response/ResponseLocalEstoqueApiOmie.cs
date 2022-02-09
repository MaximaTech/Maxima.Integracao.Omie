using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseLocalEstoqueApiOmie : ResponseOmiePorN
    {
        public ResponseLocalEstoqueApiOmie()
        {
            LocaisEstoque = new List<LocalEstoqueOmie>();
        }
        [JsonProperty("locaisEncontrados")]
        public List<LocalEstoqueOmie> LocaisEstoque { get; set; }
    }
}
