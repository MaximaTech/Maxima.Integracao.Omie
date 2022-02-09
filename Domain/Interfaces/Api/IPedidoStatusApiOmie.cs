using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Maxima.Cliente.Omie.Domain.Api.Response;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IPedidoStatusApiOmie
    {
        [JobDisplayName("Atualizar status dos pedidos")]
        Task SincronizarStatusPedidosHistorico(CancellationToken token);
        Task StatusPedidosPubSub(ResponseStatusPedidoOmie statusPedidoOmie);
    }
}
