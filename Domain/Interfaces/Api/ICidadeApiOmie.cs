using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface ICidadeApiOmie
    {
        [JobDisplayName("Atualizar Cidades")]
        Task EnviarCidades(CancellationToken token);
    }
}
