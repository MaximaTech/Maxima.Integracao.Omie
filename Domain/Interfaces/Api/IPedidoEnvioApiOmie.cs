using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Maxima.Net.SDK.Integracao.Dto.Pedido;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IPedidoEnvioApiOmie
    {
        [JobDisplayName("Enviar pedidos")]
        Task ReceberPedidos(CancellationToken token);
        Task EnviarPedidoAsync(PedidoMaxima pedidoMaxima);
    }
}
