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
        public void FormaDePagamentoMapping()
        {
            CreateMap<FormaPagamentoOmie, PlanoPagamentoMaxima>()
                .ForMember(db => db.CodigoPlanoPagamento, map => map.MapFrom(b => b.cCodigo))
                .ForMember(db => db.Descricao, map => map.MapFrom(b => b.cDescricao))
                .ForMember(db => db.QuantidadeParcelas, map => map.MapFrom(b => b.nQtdeParc))
                .AfterMap((omie, maxima) =>
                {
                    maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
                });
        }

    }
}
