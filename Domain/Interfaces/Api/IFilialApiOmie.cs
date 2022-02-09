using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IFilialApiOmie
    {
        [JobDisplayName("Atualizar Filiais")]
        Task EnviarFiliais(CancellationToken token);
    }
}
