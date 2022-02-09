using System.Collections.Generic;
using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseHistoricoPedidoOmie : ResponseOmie
    {
        public ResponseHistoricoPedidoOmie()
        {
            Pedidos = new List<PedidoOmie>();
        }
        [JsonProperty("pedido_venda_produto")]
        public List<PedidoOmie> Pedidos { get; set; }
    }
}