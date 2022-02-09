using System.Security.Cryptography;
using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Net.SDK.Integracao.Entidades;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll
    {

        public void BancoMapping()
        {
            CreateMap<BancoOmie, BancoMaxima>()
               .ForMember(db => db.Nome, map => map.MapFrom(b => b.nome))
               .ForMember(db => db.CodigoBanco, map => map.MapFrom(b => b.codigo))
               .AfterMap((omie, maxima) =>
                {
                    maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
                });


        }

    }
}