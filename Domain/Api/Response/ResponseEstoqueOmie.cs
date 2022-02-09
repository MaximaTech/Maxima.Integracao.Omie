using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseEstoqueOmie : ResponseOmiePorN
    {
        public ResponseEstoqueOmie()
        {
            EstoqueProdutos = new List<ConsultaEstoqueOmie>();
        }
        [JsonProperty("produtos")]
        public List<ConsultaEstoqueOmie> EstoqueProdutos { get; set; }
    }
}
