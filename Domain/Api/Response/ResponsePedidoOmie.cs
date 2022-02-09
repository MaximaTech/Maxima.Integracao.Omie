using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponsePedidoOmie
    {
        public ResponsePedidoOmie()
        {
            Cancelada = "N";
        }
        [JsonProperty("codigo_pedido")]
        public string CodigoPedido { get; set; }

        [JsonProperty("codigo_pedido_integracao")]
        public string CodigoPedidoIntegracao { get; set; }

        [JsonProperty("codigo_status")]
        public string CodigoStatus { get; set; }

        [JsonProperty("descricao_status")]
        public string DescricaoStatus { get; set; }

        [JsonProperty("numero_pedido")]
        public string NumeroPedido { get; set; }

        [JsonProperty("etapa")]
        public string Etapa { get; set; }

        [JsonProperty("cancelada")]
        public string Cancelada { get; set; }

        [JsonProperty("faturada")]
        public string Faturada { get; set; }

        [JsonProperty("faultcode")]
        public string faultcode { get; set; }

        [JsonProperty("faultstring")]
        public string faultstring { get; set; }

        public bool Sucesso
        {
            get
            {
                return faultcode == null && !string.IsNullOrEmpty(NumeroPedido);
            }
        }

        public bool PedidoJaIncluidoAnteriormente
        {
            get
            {
                return faultcode != null && faultcode.Equals("SOAP-ENV:Client-102");
            }
        }
    }
}