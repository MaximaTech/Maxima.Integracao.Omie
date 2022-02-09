using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IFornecedorApiOmie
    {
        [JobDisplayName("Atualizar Fornecedores")]
        Task EnviarFornecedores(CancellationToken token);
    }
}
