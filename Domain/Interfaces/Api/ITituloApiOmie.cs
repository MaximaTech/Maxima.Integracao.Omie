using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    interface ITituloApiOmie
    {
        [JobDisplayName("Atualizar Titulos")]
        Task EnviarTitulos(CancellationToken token, bool isCargaInicial);
    }
}
