using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseCategoriaOmie : ResponseOmie
    {
        public ResponseCategoriaOmie()
        {
            Categorias = new List<CategoriaOmie>(); 
        }
        [JsonProperty("categoria_cadastro")]
        public List<CategoriaOmie> Categorias { get; set; }

       
    }
}
