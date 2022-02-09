using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IDepartamentoApiOmie
    {
        [JobDisplayName("Atualizar Departamentos")]
        Task ListarDepartamentos(CancellationToken token);
    }
}