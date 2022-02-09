using System;
using System.Linq;
using System.Security.Cryptography;
using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Net.SDK.Integracao.Entidades;
using Newtonsoft.Json;
using System.Globalization;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll
    {

        public void PesquisaTituloMapping()
        {
            CreateMap<PesquisaTituloOmie, TitulosMaxima>()
                .ForMember(db => db.NumeroTransacaoVenda, map => map.MapFrom(b => b.cabecTitulo.cNumOS))
                .ForMember(db => db.NumeroDuplicata, map => map.MapFrom((b, m) =>
                {
                    if (String.IsNullOrEmpty(b.cabecTitulo.cNumDocFiscal) || b.cabecTitulo.cNumDocFiscal.Equals("null"))
                        return b.cabecTitulo.nCodTitulo.ToString();

                    var duplic = string.Join("", b.cabecTitulo.cNumDocFiscal.ToCharArray().Where(Char.IsDigit));

                    if (String.IsNullOrEmpty(duplic) || duplic.Length > 10)
                        return b.cabecTitulo.nCodTitulo.ToString();
                    else
                        return duplic;
                }))
                .ForMember(db => db.CodigoCliente, map => map.MapFrom(b => b.cabecTitulo.nCodCliente))
                .ForMember(db => db.NumeroPrestacao, map => map.MapFrom((b, m) =>
                {
                    if (b.cabecTitulo.cNumParcela != null)
                        return b.cabecTitulo.cNumParcela.Split("/")[0];
                    else
                        return "";
                }))
                .ForMember(db => db.DataEmissao, map => map.MapFrom(b => DateTime.Parse(b.cabecTitulo.dDtEmissao, new CultureInfo("pt-BR"))))
                .ForMember(db => db.DataVencimento, map => map.MapFrom(b => DateTime.Parse(b.cabecTitulo.dDtVenc, new CultureInfo("pt-BR"))))
                .ForMember(db => db.DataVencimentoOriginal, map => map.MapFrom(b => DateTime.Parse(b.cabecTitulo.dDtVenc, new CultureInfo("pt-BR"))))
                .ForMember(db => db.DataPagamento, map => map.MapFrom<DateTime?>((src, dest) =>
                {
                    if (src.cabecTitulo.dDtPagamento == null)
                        return null;
                    else
                        return DateTime.Parse(src.cabecTitulo.dDtPagamento);
                }))
                .ForMember(db => db.DataDaBaixa, map => map.MapFrom<DateTime?>((src, dest) =>
                {
                    if (src.cabecTitulo.dDtPagamento == null)
                        return null;
                    else
                        return DateTime.Parse(src.cabecTitulo.dDtPagamento);
                }))
                .ForMember(db => db.Status, map => map.MapFrom(b => retornaStatusTitulo(b.cabecTitulo.cStatus)))
                .ForMember(db => db.Valor, map => map.MapFrom(b => b.cabecTitulo.nValorTitulo))
                .ForMember(db => db.ValorOriginal, map => map.MapFrom(b => b.cabecTitulo.nValorTitulo))
                .ForMember(db => db.CodigoVendedor, map => map.MapFrom(b => b.cabecTitulo.cCodVendedor))
                .ForMember(db => db.EnviadoProtesto, map => map.MapFrom(b => "N"))
                .ForMember(db => db.EnviadoCartorio, map => map.MapFrom(b => "N"))
                .ForMember(db => db.ValorPago, map => map.MapFrom<decimal>((src, dest) =>
                {
                    if (src.lancamentos == null || src.lancamentos.Count == 0)
                        return new Decimal(0.0);
                    else
                        return src.lancamentos.Sum(s => s.nValLanc);
                }))
                .AfterMap((omie, maxima) =>
                {
                    maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
                });
        }

        private string retornaStatusTitulo(string cStatus)
        {
            switch (cStatus)
            {
                case "CANCELADO": return "C";
                case "EMABERTO": return "A";
                case "PAGTO_PARCIAL": return "A";
                case "VENCEHOJE": return "A";
                case "AVENCER": return "A";
                case "ATRASADO": return "A";
                case "LIQUIDADO": return "P";
                case "RECEBIDO": return "P";
                case "A VENCER (BOLETO GERADO)": return "A";
                default: return "A";
            }
        }    
    }
}
