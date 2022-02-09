using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IPedidoHistoricoApiOmie
    {
        [JobDisplayName("Atualizar Historico dos pedidos")]
        Task HistoricoPedidos(CancellationToken token);
    }
}