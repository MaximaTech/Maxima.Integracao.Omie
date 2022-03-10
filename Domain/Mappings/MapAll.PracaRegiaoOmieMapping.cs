using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Net.SDK.Integracao.Entidades;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll
    {

        public void TabelaDePrecos()
        {
            CreateMap<TabelaPrecosOmie, RegiaoMaxima>()
                .ForMember(db => db.CodigoRegiao, map => map.MapFrom(b => b.nCodTabPreco))
                .ForMember(db => db.NomeRegiao, map => map.MapFrom(b => b.cNome))
                .ForMember(db => db.Ativo, map => map.MapFrom(b => b.cAtiva.Equals("S") ? "A" : "I"))
                .AfterMap((omie, maxima) =>
                {
                    maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
                });

            CreateMap<TabelaPrecosOmie, PracaMaxima>()
                .ForMember(db => db.Descricao, map => map.MapFrom(b => b.cNome))
                .ForMember(db => db.CodigoPraca, map => map.MapFrom(b => b.nCodTabPreco))
                .ForMember(db => db.CodigoRegiao, map => map.MapFrom(b => b.nCodTabPreco))
                .ForMember(db => db.Ativa, map => map.MapFrom(b => b.cAtiva.Equals("S") ? "A" : "I"))
                .AfterMap((omie, maxima) =>
                {
                    maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
                });
        }
    }
}

