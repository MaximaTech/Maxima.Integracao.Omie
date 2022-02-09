using Hangfire;

namespace Maxima.Cliente.Omie.Domain.Interfaces
{
    public interface IJobs
    {
        [JobDisplayName("SN:: Recuperar Jobs do ERP selecionado")]
        void RecuperarJobsERPs();

        void CargaInicial();
    }
}
