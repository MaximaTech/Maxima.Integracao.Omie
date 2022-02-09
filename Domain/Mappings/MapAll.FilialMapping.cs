using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Net.SDK.Integracao.Entidades;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll
    {
        public void FilialMapping()
        {
            CreateMap<FilialOmie, FilialMaxima>()
                .ForMember(db => db.Bairro, map => map.MapFrom(b => b.bairro))
                .ForMember(db => db.Cep, map => map.MapFrom(b => b.cep))
                .ForMember(db => db.Cidade, map => map.MapFrom(b => b.cidade))
                .ForMember(db => db.Cnpj, map => map.MapFrom(b => b.cnpj))
                .ForMember(db => db.CodigoFilial, map => map.MapFrom(b => b.codigo_empresa))
                .ForMember(db => db.Logradouro, map => map.MapFrom(b => b.endereco))
                .ForMember(db => db.Uf, map => map.MapFrom(b => b.estado))
                .ForMember(db => db.InscricaoEstadual, map => map.MapFrom(b => b.inscricao_estadual))
                .ForMember(db => db.RazaoSocial, map => map.MapFrom(b => b.razao_social))
                .ForMember(db => db.NomeFantasia, map => map.MapFrom(b => b.nome_fantasia))
                .ForMember(db => db.Telefone, map => map.MapFrom(b => string.Concat(b.telefone1_ddd, b.telefone1_numero)))
                .AfterMap((omie, maxima) =>
                {
                    maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
                });


        }
    }
}
