using Maxima.Cliente.Omie.Domain.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maxima.Cliente.Omie.Domain.Interfaces.Api
{
    public interface ICategoriaApiOmie 
    {
        Task<List<CategoriaOmie>> RecuperarCategoria();
    }
}
