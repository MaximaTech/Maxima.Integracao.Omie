using System.Collections.Generic;
using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseVendedorOmie : ResponseOmie
    {
        public ResponseVendedorOmie()
        {
            Vendedores = new List<VendedorOmie>();
        }
        [JsonProperty("cadastro")]
        public List<VendedorOmie> Vendedores { get; set; }
    }
}
