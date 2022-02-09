using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseProdutoFornecedorOmie : ResponseOmie
    {
        public ResponseProdutoFornecedorOmie()
        {
            ProdutoxFornecedorList = new List<ProdutosPorFornecedorOmie>();
        }
        [JsonProperty("cadastros")]
        public List<ProdutosPorFornecedorOmie> ProdutoxFornecedorList { get; set; }
    }
}
