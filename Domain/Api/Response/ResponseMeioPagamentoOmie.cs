using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseMeioPagamentoOmie : ResponseOmie
    {
        public ResponseMeioPagamentoOmie()
        {
            Pagamentos = new List<MeiosPagamentoOmie>();
        }
        [JsonProperty("tipo_documento_cadastro")]
        public List<MeiosPagamentoOmie> Pagamentos { get; set; }
    }
}
