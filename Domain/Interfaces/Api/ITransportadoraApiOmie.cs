using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface ITransportadoraApiOmie
    {
        [JobDisplayName("Atualizar Transportadoras")]
        Task EnviarTransportadoras(CancellationToken token);
    }
}
