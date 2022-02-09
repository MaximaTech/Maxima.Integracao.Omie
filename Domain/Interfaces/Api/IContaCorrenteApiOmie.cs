using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface IContaCorrenteApiOmie
    {
        Task<Dictionary<string, string>> RecuperarContaCorrente();
    }
}
