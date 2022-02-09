using Maxima.Cliente.Omie.Domain.Api.Response;
using Maxima.Cliente.Omie.Domain.Interfaces.Api;
using Maxima.Net.SDK.Integracao.Api;
using Maxima.Net.SDK.Integracao.Dto.Pedido;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Work
{
    public class WorkPubSubMaxima
    {
        private readonly IPedidoEnvioApiOmie pedidoEvioApiOmie;
        private readonly IPedidoStatusApiOmie statusApiOmie;
        private readonly MaximaIntegracao apiMaxima;

        public WorkPubSubMaxima(MaximaIntegracao maximaIntegracao, IPedidoEnvioApiOmie pedidoEvioApiOmie, IPedidoStatusApiOmie statusApiOmie)
        {
            this.apiMaxima = maximaIntegracao;
            this.pedidoEvioApiOmie = pedidoEvioApiOmie;
            this.statusApiOmie = statusApiOmie;
        }

        public void IniciarOuvintes()
        {
            apiMaxima.OnIncluirPedido = IncluirPedidoMaxima;
            apiMaxima.OnStatusPedido = StatusPedidoOmie;
        }

        public async void IncluirPedidoMaxima(PedidoMaxima pedidoMaxima)
        {
            await pedidoEvioApiOmie.EnviarPedidoAsync(pedidoMaxima);

        }

        public async void StatusPedidoOmie(string statusPedidoOmieJson)
        {

            ResponseStatusPedidoOmie statusPedidoOmie = JsonConvert.DeserializeObject<ResponseStatusPedidoOmie>(statusPedidoOmieJson);
            if (statusPedidoOmie.Topic != null)
            {
                if (statusPedidoOmie.Topic.Equals("VendaProduto.EtapaAlterada")
                || statusPedidoOmie.Topic.Equals("VendaProduto.Cancelada")
                || statusPedidoOmie.Topic.Equals("VendaProduto.Devolvida")
                || statusPedidoOmie.Topic.Equals("VendaProduto.Excluida"))
                {
                    await statusApiOmie.StatusPedidosPubSub(statusPedidoOmie);
                }
            }
        }
    }
}