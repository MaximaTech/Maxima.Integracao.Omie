using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Net.SDK.Integracao.Dto.Pedido;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll
    {
        public void ClienteMaximaMap()
        {
            CreateMap<ClienteMaximaCadastro, ClienteOmie>()
                //.ForMember(o => o.codigo_cliente_omie, map => map.MapFrom(s => s.CodigoClienteNuvem))
                .ForMember(o => o.codigo_cliente_integracao, map => map.MapFrom(s => s.CodigoClienteNuvem))
                .ForMember(o => o.razao_social, map => map.MapFrom(s => s.Nome))
                .ForMember(o => o.nome_fantasia, map => map.MapFrom(s => s.Fantasia))
                .ForMember(o => o.cnpj_cpf, map => map.MapFrom(s => s.CNPJ))
                .ForMember(o => o.inscricao_estadual, map => map.MapFrom(s => s.InscricaoEstadual))
                .ForMember(o => o.inscricao_municipal, map => map.MapFrom(s => s.InscricaoMunicipal))
                .ForMember(o => o.fax_numero, map => map.MapFrom(s => s.Fax))
                .ForMember(o => o.bairro, map => map.MapFrom(s => s.EnderecoComercial.Bairro))
                .ForMember(o => o.cidade, map => map.MapFrom(s => s.EnderecoComercial.Cidade))
                .ForMember(o => o.cep, map => map.MapFrom(s => s.EnderecoComercial.Cep))
                .ForMember(o => o.complemento, map => map.MapFrom(s => s.EnderecoComercial.Complemento))
                .ForMember(o => o.estado, map => map.MapFrom(s => s.EnderecoComercial.UF))
                .ForMember(o => o.endereco, map => map.MapFrom(s => s.EnderecoComercial.Logradouro))
                .ForMember(o => o.homepage, map => map.MapFrom(s => s.Site))
                .ForMember(o => o.tipo_atividade, map => map.MapFrom(s => s.RamoAtividade.Codigo))
                .ForMember(o => o.bloqueado, map => map.MapFrom(s => "N"))
                .ForMember(o => o.enderecoEntrega, map => map.MapFrom<EnderecoEntrega>((m, o) =>
                {
                    var enderecoEntregaOmie = new EnderecoEntrega();
                    var endereroEntregaMaxima = m.Endereco;
                    if (endereroEntregaMaxima != null)
                    {
                        enderecoEntregaOmie.entBairro = endereroEntregaMaxima.Bairro;
                        enderecoEntregaOmie.entCEP = endereroEntregaMaxima.Cep;
                        enderecoEntregaOmie.entCidade = endereroEntregaMaxima.Cidade;
                        enderecoEntregaOmie.entCnpjCpf = m.CNPJ;
                        enderecoEntregaOmie.entComplemento = endereroEntregaMaxima.Complemento;
                        enderecoEntregaOmie.entEndereco = endereroEntregaMaxima.Logradouro;
                        enderecoEntregaOmie.entEstado = endereroEntregaMaxima.UF;
                        enderecoEntregaOmie.entNumero = endereroEntregaMaxima.Numero;
                    }
                    return enderecoEntregaOmie;
                }))
               .ForMember(o => o.recomendacoes, map => map.MapFrom<Recomendacoes>((m, o) =>
               {
                   var recomendacoesOmie = new Recomendacoes();
                   recomendacoesOmie.codigo_vendedor = m.CodigoRCA;
                   recomendacoesOmie.email_fatura = m.EmailNFE;
                   recomendacoesOmie.numero_parcelas = m.PlanoPagamento?.Codigo;
                   return recomendacoesOmie;
               }))
                .ForMember(o => o.tags, map => map.MapFrom(s => new List<Tag>() { new Tag() { tag = "Cliente" } }))
                .ForMember(o => o.telefone1_numero, map => map.MapFrom(s => s.Telefone))
                .ForMember(o => o.email, map => map.MapFrom(s => s.Email))
                .ForMember(o => o.observacao, map => map.MapFrom(s => s.Observacao))
                .ForMember(o => o.pessoa_fisica, map => map.MapFrom(s => s.PessoaFisica.Equals("true") ? "S" : "N"))
                ;

        }

    }
}
