using Hangfire;
using Hangfire.Server;
using Maxima.Cliente.Omie.Data.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface IConfiguracao
    {
        Task<bool> Salvar(ParametroModel config);

        Task<bool> Alterar(ParametroModel config);

        Task<bool> Excluir(int id);

        Task<ParametroModel> Buscar(int id = 0, string nome = "");

        Task ProximaEtapaConfig();
        string EtapaAtual();
        Task<bool> AutenticarOmie(string appKey, string appSecret);
        string ConvertToCron(string hora, string minuto);
        Task FinalizarCargaInicialOmie(PerformContext performContext, CancellationToken token);
        Task CargaInicialErro();
        Task VoltarCargaInicial();

    }
}
