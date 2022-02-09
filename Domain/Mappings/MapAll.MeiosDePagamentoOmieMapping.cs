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
        public void MeiosDePagamentoOmieMapping()
        {
            CreateMap<MeiosPagamentoOmie, CobrancaMaxima>()
                .ForMember(db => db.CodigoCobranca, map => map.MapFrom(b => b.codigo))
                .ForMember(db => db.NomeCobranca, map => map.MapFrom(b => b.descricao))
                .AfterMap((omie, maxima) =>
                {
                    maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
                });
        }
    }
}
