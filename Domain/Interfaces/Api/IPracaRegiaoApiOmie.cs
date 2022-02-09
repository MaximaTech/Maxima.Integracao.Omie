using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IPracaRegiaoApiOmie
    {
        [JobDisplayName("Atualizar Praças e Regiões")]
        Task EnviarPracaRegiao(CancellationToken token);
    }
}
