namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class FilialOmie
    {
        public long codigo_empresa { get; set; }                             //	Código da Empresa
        public string codigo_empresa_integracao { get; set; }               //	Código de Integração
        public string cnpj { get; set; }                                    //	CNPJ da Empresa
        public string razao_social { get; set; }                            //	Razão Social
        public string nome_fantasia { get; set; }                           //	Nome Fantasia
        public string logradouro { get; set; }                              //	Logradouro
        public string endereco { get; set; }                                //	Endereço da Empresa
        public string endereco_numero { get; set; }                         //	Número do Endereço
        public string complemento { get; set; }                             //	Complemento do Número do Endereço
        public string bairro { get; set; }                                  //	Bairro
        public string cidade { get; set; }                                  //	Código da Cidade
        public string estado { get; set; }                                  //	Estado da Empresa
        public string cep { get; set; }                                     //	CEP
        public string codigo_pais { get; set; }                             //	Código do País
        public string data_adesao_sn { get; set; }                          //	Data de Adesão ao Simples Nacional
        public string telefone1_ddd { get; set; }                           //	DDD
        public string telefone1_numero { get; set; }                        //	Telefones da Empresa
        public string telefone2_ddd { get; set; }                           //	DDD Telefone 2
        public string telefone2_numero { get; set; }                        //	Telefone 2
        public string fax_ddd { get; set; }                                 //	DDD Fax
        public string fax_numero { get; set; }                              //	Fax da Empresa
        public string email { get; set; }                                   //	E-mail da Empresa
        public string website { get; set; }                                 //	WebSite
        public string cnae { get; set; }                                    //	Código do CNAE - Fiscal
        public string cnae_municipal { get; set; }                          //	Código do CNAE - Municipal
        public string inscricao_estadual { get; set; }                      //	Inscrição Estadual
        public string inscricao_municipal { get; set; }                     //	Inscrição Municipal
        public string inscricao_suframa { get; set; }                       //	Inscrição no SUFRAMA
        public string regime_tributario { get; set; }                       //	Regime Tributário
        public string inativa { get; set; }                                 //	Indica que a empresa está ativa
        public string gera_nfse { get; set; }                               //	Gera Nota Fiscal de Serviço Eletrônica para o Município [S/N]
        public string optante_simples_nacional { get; set; }                //	Indica que a empresa é optante pelo Simples Nacional
        public string sped_codigo_incidencia_tributaria { get; set; }       //	SPED PIS/COFINS - Código Indicador da Incidência Tributária no Período
        public string sped_codigo_tipo_contribuicao { get; set; }           //	SPED PIS/COFINS - Código indicador do Tipo de Contribuição Apurada no Período
        public string sped_cpf_contador { get; set; }                       //	SPED - CPF do Contador Responsável pela Empresa
        public string sped_crc_contador { get; set; }                       //	SPED - CRC do Contador Responsável pela Empresa
        public string sped_usa_contabilidade_terceirizada { get; set; }     //	SPED - Indica se a Empresa utiliza Contabilidade Terceirizada
        public string sped_email_contador { get; set; }                     //	SPED - Email do Contador da Empresa
        public string sped_codigo_indicador_apropriacao_credito { get; set; }  //	SPED PIS/COFINS - Código Código Indicador de Método de Apropriação de Créditos Comuns
        public string sped_codigo_tipo_atividade { get; set; }              //	SPED PIS/COFINS - Código Indicador de Tipo de Atividade Preponderante
        public string sped_natureza_pessoa_juridica { get; set; }           //	SPED PIS/COFINS - Indicador da Natureza da Pessoa Jurídica
        public string sped_codigo_criterio_escrituracao { get; set; }       //	SPED PIS/COFINS - Código Indicador do Critério de Escrituração e Apuração
        public string sped_junta_comercial { get; set; }                    //	SPED - Indica se o Local do Registro é Junta Comercial
        public string sped_matriz { get; set; }                             //	SPED - Indica se a empresa é Matriz
        public string sped_nome_contador { get; set; }                      //	SPED - Nome do Contador Responsável pela Empresa
        public string sped_registro_junta_comercial { get; set; }           //	SPED - Número do Registro da Empresa na Junta Comercial ou no Cartório
        public string efd_atividade_industrial { get; set; }                //	EFD - Indica se a atividade é industrial ou equiparado a indústria
        public string efd_perfil_arquivo_fiscal { get; set; }               //	Perfil de Apresentação do Arquivo Fiscal - EFD
        public string ecd_codigo_cadastral { get; set; }                    //	SPED / ECD - Código cadastral na Instituição Responsável
        public string ecd_codigo_instituicao_responsavel { get; set; }      //	SPED / ECD - Código da Instituição Responsável pela Administração do Cadastro
        public string gera_nfe { get; set; }                                //	Gera Nota Fiscal Eletrônica Âmbito Nacional (DANFE) [S/N]
        public string inclusao_data { get; set; }                           //	Data de inclusão
        public string inclusao_hora { get; set; }                           //	Hora de inclusão.
        public string alteracao_data { get; set; }                          //	Data de alteração
        public string alteracao_hora { get; set; }                          //	Hora de alteração
        public string bloqueado { get; set; }                               //	Registro bloqueado pela API
        public string importado_api { get; set; }                           //	Importado da API.
        public string pdv_sincr_analitica { get; set; }                     //	Sincronizar Estoque de Forma Análitica (Venda a Venda)
        public string dt_adesao_simpnac { get; set; }                       //	Data de Adesão do Simples Nacional
        public string ie_st { get; set; }                                   //	Inscrição Estadual para ST
        public string recibo_ide { get; set; }                              //	Identificação do Profissional (contador, advogado, etc) para o Recibo de Prestação de Serviço
        public string gera_recibo { get; set; }                             //	Gera Recibo de Prestação de Serviço [S/N]
        public string recibo_tipo { get; set; }                             //	Tipo (modelo) do Recibo de Prestação de Serviço
        public string recibo_num_vias { get; set; }                         //	Indica de o Recibo de Prestação de Serviço será gerado em 2 vias

        public const string UrlApi = "https://app.omie.com.br/api/v1/geral/empresas/";
        public const string VersaoAPI = "v1/geral/";
        public const string Endpoint = "empresas/";
    }
}
