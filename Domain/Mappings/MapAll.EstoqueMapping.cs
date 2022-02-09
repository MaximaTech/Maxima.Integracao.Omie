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
        public void EstoqueMapping()
        {
            CreateMap<ConsultaEstoqueOmie, EstoqueMaxima>()
            .ForMember(db => db.CodigoProduto, map => map.MapFrom(b => b.nCodProd))
            .ForMember(db => db.QtdEstoqueGerencial, map => map.MapFrom(b => b.nSaldo))
            .ForMember(db => db.CustoFinanceiro, map => map.MapFrom(b => b.nPrecoUnitario))
            .ForMember(db => db.CustoReal, map => map.MapFrom(b => b.nPrecoUnitario))
            .ForMember(db => db.CustoReposicao, map => map.MapFrom(b => b.nPrecoUnitario))
            .AfterMap((omie, maxima) =>
            {
                maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
            });

        }
    }
}
