using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Net.SDK.Integracao.Entidades;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll
    {
        public void VendedorMapping()
        {
            CreateMap<VendedorOmie, VendedorMaxima>()
                .ForMember(db => db.CodigoVendedor, map => map.MapFrom(b => b.codigo))
                .ForMember(db => db.Nome, map => map.MapFrom(b => b.nome))
                .ForMember(db => db.Bloqueio, map => map.MapFrom(b => b.inativo))
                .ForMember(db => db.Email, map => map.MapFrom(b => b.email))
                .AfterMap((omie, maxima) =>
                {
                    maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
                });
            ;

        }
    }
}
