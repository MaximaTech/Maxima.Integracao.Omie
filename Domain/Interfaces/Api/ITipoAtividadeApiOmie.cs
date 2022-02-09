using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface ITipoAtividadeApiOmie
    {
        [JobDisplayName("Atualizar Tipos Atividades")]
        Task EnviarTipoAtividade(CancellationToken token);
    }
}
