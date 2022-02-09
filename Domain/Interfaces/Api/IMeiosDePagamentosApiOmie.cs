using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Maxima.Cliente.Omie.Domain.Entidades;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IMeiosDePagamentosApiOmie
    {
        Task<List<MeiosDePagamentoOmie>> RecuperarMeiosdePagamentos();
        [JobDisplayName("Atualizar Meios de Pagamento")]
        Task EnviarMeiosPagamento(CancellationToken token);

    }
}
