using System;
using System.Globalization;
using System.Security.Cryptography;
using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Net.SDK.Integracao.Entidades;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll
    {

        public void ProdutoMapping()
        {
            CreateMap<ProdutoOmie, ProdutoMaxima>()
                 .ForMember(o => o.CodigoProduto, map => map.MapFrom(s => s.codigo_produto))
                 .ForMember(o => o.CodigoProdutoPrincipal, map => map.MapFrom(s => s.codigo_produto))
                 .ForMember(o => o.CodigoFabrica, map => map.MapFrom(s => s.codigo))
                 .ForMember(o => o.PesoLiquido, map => map.MapFrom(s => s.peso_liq == 0 ? 1 : s.peso_liq))
                 .ForMember(o => o.PesoBruto, map => map.MapFrom(s => s.peso_bruto == 0 ? 1 : s.peso_bruto))
                 .ForMember(o => o.Unidade, map => map.MapFrom(s => s.unidade))
                 .ForMember(o => o.Ncm, map => map.MapFrom(s => s.ncm))
                 .ForMember(o => o.CodigoDeBarras, map => map.MapFrom(s => s.ean))
                 .ForMember(o => o.Descricao, map => map.MapFrom(s => s.descricao))
                 .ForMember(o => o.DescricaoSecundaria, map => map.MapFrom(s => s.descr_detalhada))
                 .ForMember(o => o.DataCadastro, map => map.MapFrom(s => DateTime.Parse(s.info.dAlt, new CultureInfo("pt-BR"))))
                 .AfterMap((omie, maxima) =>
                 {
                    maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
                 });

        }

    }
}