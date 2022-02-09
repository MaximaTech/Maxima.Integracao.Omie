using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Net.SDK.Integracao.Entidades;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll
    {
        public void CidadeMapping()
        {
            CreateMap<CidadeOmie, CidadeMaxima>()
                .ForMember(dc => dc.NomeCidade, map => map.MapFrom(c => c.cNome))
                .ForMember(dc => dc.UF, map => map.MapFrom(c => c.cUF))
                .ForMember(dc => dc.CodigoCidadeIBGE, map => map.MapFrom(c => c.nCodIBGE))
                .ForMember(dc => dc.CodigoCidade, map => map.MapFrom(c => c.cCod))
                .AfterMap((omie, maxima) =>
                {
                    maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
                });

            // Necessario
            //      CODCIDADE

            //Lado De Ca
            //.ForMember(dc => dc.Codcidade, map => map.MapFrom(c => c.))
            //.ForMember(dc => dc.Hash, map => map.MapFrom(c => c.))

            //lado de la 
            //.ForMember(dc => dc., map => map.MapFrom(c => c.cCod))
            //.ForMember(dc => dc., map => map.MapFrom(c => c.nCodSIAFI));
        }
    }
}
