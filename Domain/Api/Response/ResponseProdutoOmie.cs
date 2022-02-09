using System.Collections.Generic;
using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseProdutoOmie : ResponseOmie
    {
        public ResponseProdutoOmie()
        {
            Produtos = new List<ProdutoOmie>();
            Vendedores = new List<VendedorOmie>();
        }
        [JsonProperty("produto_servico_cadastro")]
        public List<ProdutoOmie> Produtos { get; set; }

        [JsonProperty("cadastro")]
        public List<VendedorOmie> Vendedores { get; set; }
    }
}