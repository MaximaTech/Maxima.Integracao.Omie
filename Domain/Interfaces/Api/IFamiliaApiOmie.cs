using Hangfire;
using System.Threading;
using System.Threading.Tasks;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IFamiliaApiOmie
    {
        [JobDisplayName("Atualizar Familias")]
        Task ListarFamilias(CancellationToken token);
    }
}
