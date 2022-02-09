using System.Collections.Generic;
using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponsePesquisaTituloOmie : ResponseOmiePorN
    {
        public ResponsePesquisaTituloOmie()
        {
            Titulos = new List<PesquisaTituloOmie>();
        }
        [JsonProperty("titulosEncontrados")]
        public List<PesquisaTituloOmie> Titulos { get; set; }
    }
}
