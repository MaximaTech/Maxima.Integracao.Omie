using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseFormaPagamento : ResponseOmie
    {
        public ResponseFormaPagamento()
        {
            FormaPagamentos = new List<FormaPagamentoOmie>();
        }
        [JsonProperty("cadastros")]
        public List<FormaPagamentoOmie> FormaPagamentos { get; set; }

    }
}
