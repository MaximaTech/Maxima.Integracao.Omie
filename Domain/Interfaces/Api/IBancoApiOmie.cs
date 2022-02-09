using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IBancoApiOmie
    {
        [JobDisplayName("Atualizar Bancos")]
        Task EnviarBancos(CancellationToken token);
    }
}
