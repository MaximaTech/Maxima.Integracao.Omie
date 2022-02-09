using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IPrecoProdutoApiOmie
    {
        [JobDisplayName("Atualizar Precos")]
        Task EnviarPrecoProdutos(CancellationToken token);
    }
}
