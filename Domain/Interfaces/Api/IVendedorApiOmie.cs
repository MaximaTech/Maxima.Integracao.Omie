using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IVendedorApiOmie
    {
        [JobDisplayName("Atualizar Vendedores")]
        Task EnviarVendedores(CancellationToken token);
    }
}
