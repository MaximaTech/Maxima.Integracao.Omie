using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Net.SDK.Integracao.Entidades;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll
    {
        public void TransportadoraMapping()
        {
            CreateMap<ClienteOmie, TransportadoraMaxima>()
               .ForMember(db => db.CodigoTrasportadora, map => map.MapFrom(b => b.codigo_cliente_omie))
               .ForMember(db => db.CpfCnpj, map => map.MapFrom(b => b.cnpj_cpf))
               .ForMember(db => db.NomeFantasia, map => map.MapFrom(b => b.nome_fantasia))
               .ForMember(db => db.Bairro, map => map.MapFrom(b => b.bairro))
               .ForMember(db => db.CodigoCidade, map => map.MapFrom(b => b.cidade))
               .ForMember(db => db.Uf, map => map.MapFrom(b => b.estado))
               .ForMember(db => db.NomeTransportadora, map => map.MapFrom(b => b.razao_social))
               .ForMember(db => db.Ativo, map => map.MapFrom(b => b.inativo))
               .ForMember(db => db.Logradouro, map => map.MapFrom(b => b.endereco))
               .AfterMap((omie, maxima) =>
                {
                    maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
                });
        }
    }
}
