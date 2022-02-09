namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class TransportadoraOmie
    {
        public long codigo_cliente_omie { get; set; }
        public string codigo_cliente_integracao { get; set; }
        public string codPraca { get; set; }
        public string razao_social { get; set; }
        public string cnpj_cpf { get; set; }
        public string nome_fantasia { get; set; }
        public string telefone1_ddd { get; set; }
        public string telefone1_numero { get; set; }
        public string contato { get; set; }
        public string endereco { get; set; }
        public string endereco_numero { get; set; }
        public string bairro { get; set; }
        public string complemento { get; set; }
        public string estado { get; set; }
        public string cidade { get; set; }
        public string cep { get; set; }
        public string codigo_pais { get; set; }
        public string telefone2_ddd { get; set; }
        public string telefone2_numero { get; set; }
        public string fax_ddd { get; set; }
        public string fax_numero { get; set; }
        public string email { get; set; }
        public string homepage { get; set; }
        public string inscricao_estadual { get; set; }
        public string inscricao_municipal { get; set; }
        public string inscricao_suframa { get; set; }
        public string optante_simples_nacional { get; set; }
        public string tipo_atividade { get; set; }
        public string cnae { get; set; }
        public string produtor_rural { get; set; }
        public string contribuinte { get; set; }
        public string observacao { get; set; }
        public string obs_detalhadas { get; set; }
        public string recomendacao_atraso { get; set; }
        public Tag[] tags { get; set; }
        public string pessoa_fisica { get; set; }
        public string exterior { get; set; }
        public string logradouro { get; set; }
        public string importado_api { get; set; }
        public string bloqueado { get; set; }
        public string cidade_ibge { get; set; }
        public decimal valor_limite_credito { get; set; }
        public string bloquear_faturamento { get; set; }
        public Recomendacoes recomendacoes { get; set; }
        public EnderecoEntrega enderecoEntrega { get; set; }
        public string nif { get; set; }
        public string inativo { get; set; }
        public DadosBancarios dadosBancarios { get; set; }
        public Info info { get; set; }
        public string bloquear_exclusao { get; set; }

        public const string UrlApi = "https://app.omie.com.br/api/v1/geral/clientes/";
    }
}
