using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Net.SDK.Integracao.Entidades;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll
    {
        public void PrecoProdutoOmieMapping() 
        {
            CreateMap<ItensTabela, TabelaPrecoMaxima>()
                .ForMember(db => db.CodigoProduto, map => map.MapFrom(b => b.nCodProd))
                .ForMember(db => db.PrecoVendaSemImpostos, map => map.MapFrom(b => b.nValorTabela))
                .ForMember(db => db.PrecoVendaComImpostos, map => map.MapFrom(b => b.nValorTabela))
                .ForMember(db => db.PrecoTabela, map => map.MapFrom(b => b.nValorTabela))
                .AfterMap((omie, maxima) =>
                {
                    maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
                });


        }
    }
}
