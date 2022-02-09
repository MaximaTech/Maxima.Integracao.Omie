using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseTabeladePrecoOmie : ResponseOmiePorN
    {
        public ResponseTabeladePrecoOmie()
        {
            TabelasPreco = new List<TabelaPrecosOmie>();
        }
        [JsonProperty("listaTabelasPreco")]
        public List<TabelaPrecosOmie> TabelasPreco { get; set; }

        [JsonProperty("listaTabelaPreco")]
        public TabelaPrecosOmie TabelasPrecoItens { get; set; }
    }
}
