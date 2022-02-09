using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Net.SDK.Integracao.Entidades;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll
    {
        public void FamiliaMapping()
        {
            CreateMap<FamiliaOmie, FamiliaMaxima>()
            .ForMember(db => db.CodigoFamilia, map => map.MapFrom(b => b.codigo))
            .ForMember(db => db.Descricao, map => map.MapFrom(b => b.nomeFamilia))
            .AfterMap((omie, maxima) =>
            {
                maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
            });
        }
    }
}
