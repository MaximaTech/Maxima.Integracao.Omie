using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IFormaPagamentoApiOmie
    {
        [JobDisplayName("Atualizar Formas de Pagamento")]
        Task EnviarFormaDePagamento(CancellationToken token);
    }
}
