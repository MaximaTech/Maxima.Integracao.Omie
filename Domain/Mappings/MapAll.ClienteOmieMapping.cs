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

        public void ClienteMapping()
        {
            CreateMap<ClienteOmie, ClienteMaxima>()
           .ForMember(o => o.BloqueioVenda, map => map.MapFrom(s => s.bloquear_faturamento))
           .ForMember(o => o.BairroEnderecoComercial, map => map.MapFrom(s => s.bairro))
           .ForMember(o => o.BairroEnderecoEntrega, map => map.MapFrom(s => s.bairro))
           .ForMember(o => o.EnderecoComercial, map => map.MapFrom(s => s.endereco))
           .ForMember(o => o.EnderecoEntrega, map => map.MapFrom(s => s.endereco))
           .ForMember(o => o.CEPEnderecoComercial, map => map.MapFrom(s => s.cep))
           .ForMember(o => o.CEPEnderecoEntrega, map => map.MapFrom(s => s.cep))
           .ForMember(o => o.CodigoCidadeEndereco, map => map.MapFrom(s => s.cidade))
           .ForMember(o => o.MunicipioEnderecoComercial, map => map.MapFrom(s => s.cidade))
           .ForMember(o => o.MunicipioEnderecoEntrega, map => map.MapFrom(s => s.cidade))
           .ForMember(o => o.PaisEnderecoCliente, map => map.MapFrom(s => s.codigo_pais))
           .ForMember(o => o.NumeroEnderecoComercial, map => map.MapFrom(s => s.endereco_numero))
           .ForMember(o => o.NumeroEnderecoEntrega, map => map.MapFrom(s => s.endereco_numero))
           .ForMember(o => o.EstadoEnderecoComercial, map => map.MapFrom(s => s.estado))
           .ForMember(o => o.EstadoEnderecoEntrega, map => map.MapFrom(s => s.estado))
           .ForMember(o => o.ComplementoEnderecoComercial, map => map.MapFrom(s => s.complemento))
           .ForMember(o => o.ComplementoEnderecoEntrega, map => map.MapFrom(s => s.complemento))
           .ForMember(o => o.CodigoPraca, map => map.MapFrom(s => s.codPraca))
           .ForMember(o => o.CodigoVendedor1, map => map.MapFrom(s => s.recomendacoes.codigo_vendedor))
           .ForMember(o => o.CpfCnpj, map => map.MapFrom(s => s.cnpj_cpf))
           .ForMember(o => o.Email, map => map.MapFrom(s => s.email))
           .ForMember(o => o.Inativo, map => map.MapFrom(s => s.inativo))
           .ForMember(o => o.Telefone, map => map.MapFrom(s => s.telefone1_ddd + s.telefone1_numero))
           .ForMember(o => o.InscricaoEstadual, map => map.MapFrom(s => string.IsNullOrEmpty(s.inscricao_estadual) ? "ISENTO" : s.inscricao_estadual))
           .ForMember(o => o.InscricaoMunicipal, map => map.MapFrom(s => s.inscricao_municipal))
           .ForMember(o => o.CodigoRamoAtividade, map => map.MapFrom(s => String.IsNullOrEmpty(s.tipo_atividade) ? "4" : s.tipo_atividade))
           .ForMember(o => o.Telefone, map => map.MapFrom(s => s.telefone2_ddd + s.telefone2_numero))
           .ForMember(o => o.NomeFantasia, map => map.MapFrom(s => s.nome_fantasia))
           .ForMember(o => o.Obs, map => map.MapFrom(s => s.observacao))
           .ForMember(o => o.TipoCadastroCliente, map => map.MapFrom(s => s.pessoa_fisica.Equals("S") ? "F" : "J"))
           .ForMember(o => o.NomeCliente, map => map.MapFrom(s => s.razao_social))
           .ForMember(o => o.CodigoCliente, map => map.MapFrom(s => s.codigo_cliente_omie))
           .ForMember(o => o.ConsumidorFinal, map => map.MapFrom(s => s.pessoa_fisica))
           .ForMember(o => o.EmailNfe, map => map.MapFrom(s => s.email))
           .ForMember(o => o.BloqueioSefaz, map => map.MapFrom(s => s.bloquear_faturamento.Equals("S") ? "1" : "0"))
           .ForMember(o => o.LimiteCredito, map => map.MapFrom(s => s.valor_limite_credito))
           .ForMember(o => o.CodigoPlanoDePagamento, map => map.MapFrom((b, m) =>
            {
                if (b.recomendacoes != null && b.recomendacoes.numero_parcelas != null)
                    return b.recomendacoes.numero_parcelas;
                else
                    return "000";
            })).AfterMap((omie, maxima) =>
            {
                maxima.Hash = Utils.UtilsApi.GerarHashMD5(maxima);
            });

        }

    }
}