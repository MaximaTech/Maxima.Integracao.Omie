using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IProdutosPorFornecedorApiOmie
    {
        [JobDisplayName("Atualizar Produtor x Fornecedor")]

        Task EnviarProdutosPorFornecedor(CancellationToken token);
    }
}
