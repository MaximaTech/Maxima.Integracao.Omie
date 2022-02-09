using System.Collections.Generic;
using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseTipoAtividadeOmie : ResponseOmie
    {
        public ResponseTipoAtividadeOmie()
        {
            TiposAtividades = new List<TipoAtividadeOmie>();
        }
        [JsonProperty("lista_tipos_atividade")]
        public List<TipoAtividadeOmie> TiposAtividades { get; set; }
    }
}
