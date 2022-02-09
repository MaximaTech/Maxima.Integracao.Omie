using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Parameters
{
    public class ParamStatusPedido
    {
        [JsonProperty("codigo_pedido_integracao")]
        public string CodigoPedidoIntegracao { get; set; }

        [JsonProperty("codigo_pedido")]
        public string CodigoPedido { get; set; }
    }
}