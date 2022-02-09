using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Net.SDK.Integracao.Entidades;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll
    {
        public void FornecedorMapping()
        {
            CreateMap<ClienteOmie, FornecedorMaxima>()
                .ForMember(o => o.Bairro, map => map.MapFrom(s => s.bairro))
                .ForMember(o => o.NomeFantasia, map => map.MapFrom(s => s.nome_fantasia))
                .ForMember(o => o.Logradouro, map => map.MapFrom(s => s.endereco))
                .ForMember(o => o.Estado, map => map.MapFrom(s => s.estado))
                .ForMember(o => o.Cidade, map => map.MapFrom(s => s.cidade))
                .ForMember(o => o.CodigoFornecedor, map => map.MapFrom(s => s.codigo_cliente_omie))
                .ForMember(o => o.CpfCnpj, map => map.MapFrom(s => s.cnpj_cpf))
                .ForMember(o => o.NomeFornecedor, map => map.MapFrom(s => s.razao_social))
                .ForMember(o => o.Ativo, map => map.MapFrom(s => s.inativo))
                .AfterMap((omie, maxima) =>
                {
                    maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
                });
        }
    }
}
