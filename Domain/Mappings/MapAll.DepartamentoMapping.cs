using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Net.SDK.Integracao.Entidades;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll
    {
        public void DepartamentoMapping()
        {
            CreateMap<DepartamentoOmie, DepartamentoMaxima>()
               .ForMember(dc => dc.CodigoDepartamento, map => map.MapFrom(c => c.codigo)) // validar se não é pk
               .ForMember(dc => dc.Descricao, map => map.MapFrom(c => c.descricao));

            // necessarios 
            // CODEPTO
            // DESCRICAO)

            //Lado de Cá
            //.ForMember(dc => dc.Hash, map => map.MapFrom(c => c.))

            // Lado De Lá
            //.ForMember(dc => dc., map => map.MapFrom(c => c.estrutura))
            //.ForMember(dc => dc., map => map.MapFrom(c => c.inativo))
            //.ForMember(dc => dc., map => map.MapFrom(c => c.nivel_totalizador))
        }
    }
}
