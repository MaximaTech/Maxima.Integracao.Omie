namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseStatusPedidoOmie
    {

        public string Topic { get; set; }
        public ResponseStatusPedidoOmie()
        {
            Event = new Event();
        }
        public Event Event { get; set; }
    }

    public class Event
    {
        public Event()
        {
            Cancelada = "N";
        }
        public string CodIntPedido { get; set; }
        public string Etapa { get; set; }
        public string EtapaDescr { get; set; }
        public string NumeroPedido { get; set; }
        public string Cancelada { get; set; }
    }
}