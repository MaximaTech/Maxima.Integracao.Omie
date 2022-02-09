using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IClienteApiOmie
    {
        [JobDisplayName("Atualizar Clientes")]
        Task EnviarClientes(CancellationToken token);
        [JobDisplayName("Enviar Clientes")]
        Task ReceberClientes(CancellationToken token);
    }
}
