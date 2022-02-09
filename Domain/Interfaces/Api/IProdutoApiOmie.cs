using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IProdutoApiOmie
    {
        [JobDisplayName("Atualizar Produtos")]
        Task EnviarProdutos(CancellationToken token);
    }
}
