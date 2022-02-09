using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Net.SDK.Integracao.Entidades;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll
    {
        public void TipoAtividadeMapping()
        {
            CreateMap<TipoAtividadeOmie, RamoAtividadeMaxima>()
               .ForMember(db => db.CodigoRamoAtividadeEconomica, map => map.MapFrom(b => b.cCodigo))
               .ForMember(db => db.NomeRamoAtividadeEconomica, map => map.MapFrom(b => b.cDescricao))
               .AfterMap((omie, maxima) =>
                {
                    maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
                });


            /*
               //necessarios
                 RAMO
                 PERCDESC
                 CALCULAST


               //lado de ca
               .ForMember(db => db.Calculast, map => map.MapFrom(b => b.))
               .ForMember(db => db.Hash, map => map.MapFrom(b => b.))
               .ForMember(db => db.Percdesc, map => map.MapFrom(b => b.))
               .ForMember(db => db.Ramo, map => map.MapFrom(b => b.))
               //lado de la
               .ForMember(db => db., map => map.MapFrom(b => b.cCodigo))
               .ForMember(db => db., map => map.MapFrom(b => b.cDescricao))
            */
        }
    }
}
