using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IEstoqueApiOmie
    {
        [JobDisplayName("Atualizar Estoque")]
        Task EnviarEstoque(CancellationToken token);
    }
}
