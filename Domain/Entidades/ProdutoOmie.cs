namespace Maxima.Cliente.Omie.Domain.Entidades
{

    public class ProdutoOmie
    {
        public long? codigo_produto { get; set; }//Código do produto.+
        public string codigo_produto_integracao { get; set; }	//Códig	{get;set;}de integração do produto.+
        public string descricao { get; set; }//Descrição do produto.+
        public string codigo { get; set; }//Código do Produto.+
        public string unidade { get; set; }//Código da Unidade.+
        public string ncm { get; set; }//Código da Nomenclatura Comum do Mercosul (NCM).+
        public string ean { get; set; }//Código EAN (GTIN - Global Trade Item Number).+
        public decimal? valor_unitario { get; set; }//Preço Unitário de Venda.+
        public long? codigo_familia { get; set; }//Código da Familia do Produto.+
        public string tipoItem { get; set; }//Código do Tipo do Item para o SPED.+
        public decimal? peso_liq { get; set; }//Peso Líquido (Kg).+
        public decimal? peso_bruto { get; set; }//Peso Bruto (Kg).+
        public decimal? altura { get; set; }//Altura (centimentos).+
        public decimal? largura { get; set; }//Largura (centimetros)+
        public decimal? profundidade { get; set; }//Profundidade (centimetros).+
        public string marca { get; set; }//Marca.+
        public int? dias_garantia { get; set; }//Dias de Garantia.+
        public int? dias_crossdocking { get; set; }//Dias de Crossdocking.+
        public string descr_detalhada { get; set; }//Descrição Detalhada para o Produto.+
        public string obs_internas { get; set; }//Observações Internas.+

        public string exibir_descricao_nfe { get; set; }//Indica se a Descrição Detalhada deve ser exibida nas Informações Adicionais do Item da NF-e (S/N).
        public string exibir_descricao_pedido { get; set; }//Indica se a Descrição Detalhada deve ser exibida na impressão do Pedido (S/N).
        public string cst_icms { get; set; }//Código da Situação Tributária do ICMS.+
        public string modalidade_icms { get; set; }//Modalidade da Base de Cálculo do ICMS.+
        public string csosn_icms { get; set; }//Código da Situação Tributária para Simples Nacional.+
        public decimal? aliquota_icms { get; set; }//Alíquota de ICMS.+
        public decimal? red_base_icms { get; set; }//Percentual de redução de base do ICMS.+
        public string motivo_deson_icms { get; set; }//Motivo da desoneração do ICMS.+
        public decimal? per_icms_fcp { get; set; }//Percentual do Fundo de Combate a Pobreza do ICMS.+
        public string codigo_beneficio { get; set; }//Código de integração da característica do produto.+
        public string cst_pis { get; set; }//Código da Situação Tributária do PIS.+
        public decimal? aliquota_pis { get; set; }//Alíquota do PIS.+
        public string cst_cofins { get; set; }//Código da Situação Tributária do COFINS.+
        public decimal? aliquota_cofins { get; set; }//Alíquota do COFINS.+
        public string cfop { get; set; }//CFOP do Produto.+

        public string codInt_familia { get; set; }//Código de Integração da Familia do Produto.+
        public string descricao_familia { get; set; }//Descrição da Familia do Produto.+
        public string bloqueado { get; set; }//Indica se o registro está bloqueado (S/N).+
        public string bloquear_exclusao { get; set; }//Indica se a exclusão do registro está bloqueada (S/N).+
        public string importado_api { get; set; }//Indica se o registro foi incluído via API (S/N).+
        public string inativo { get; set; }//Indica se o cadastro do produto está inativo (S/N).+
        public decimal? aliquota_ibpt { get; set; }//DEPRECATED.
        public string cest { get; set; }//DEPRECATED.
        public decimal? quantidade_estoque { get; set; }//DEPRECATED.
        public decimal? estoque_minimo { get; set; }//DEPRECATED.
        public Info info { get; set; }

        public const string UrlApi = "https://app.omie.com.br/api/v1/geral/produtos/";
        public const string VersaoAPI = "v1/geral/";

        public const string Endpoint = "produtos/";

    }
}








