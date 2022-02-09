using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class PedidoOmie
    {
        public const string VersaoAPI = "v1/";
        public const string Endpoint = "produtos/pedido/";


        public Cabecalho cabecalho { get; set; }                                // Informações do cabeçalho do pedido.+
        public List<Departamento> departamentos { get; set; }                   // Dados da Aba "Departamentos" do Pedido de Venda.
        public Frete frete { get; set; }                                        // Dados da Aba 'Frete e Outras Despesas' do Pedido de Venda.+
        public InformacoesAdicionais informacoes_adicionais { get; set; }       // Dados da Aba 'Informações Adicionais' do Pedido de Venda.+
        public ListaParcelas lista_parcelas { get; set; }                       // Dados da Aba 'Parcelas' do Pedido de Venda.+
        public Observacoes observacoes { get; set; }                            // Dados da Aba 'Observações' do Pedido de Venda.+
        public List<Det> det { get; set; }                                            // Dados da Aba 'Itens da Venda' do Pedido de Venda.+
        public MarketPlace market_place { get; set; }                           // Dados para o Market Place.
        public TotalPedido total_pedido { get; set; }                           // Valores totais do pedido.+
        public InfoCadastro infoCadastro { get; set; }                          // Informações complementares do pedido.+
        public Exportacao exportacao { get; set; }                              // Dados da exportacao
        public Produto produto { get; set; }                                    // Dados Produto
        public class Cabecalho
        {

            public string codigo_pedido { get; set; }                          // ID do pedido do venda.+
            public string codigo_pedido_integracao { get; set; }            // Código de integração do pedido de venda.+
            public string numero_pedido { get; set; }                       // Número do pedido de venda.+
            public string codigo_cliente { get; set; }                         // Código do cliente.+
            public string codigo_cliente_integracao { get; set; }           // Código Integração da Transportadora.+
            public string data_previsao { get; set; }                       // Data de Previsão de Faturamento.+
            public string quantidade_itens { get; set; }                       // Quantidade de Itens.+
            public string etapa { get; set; }                               // Etapa do pedido de venda.+
            public string codigo_parcela { get; set; }                      // Código da parcela/Condição de pagamento.+
            public string qtde_parcelas { get; set; }                          // Quantidade de parcelas.+
            public string origem_pedido { get; set; }                       // Origem do Pedido.+
            public string codigo_cenario_impostos { get; set; }                // Código do Cenário de Impostos.+
            public string bloqueado { get; set; }                           // Pedido Bloqueado pela API.+
            public string importado_api { get; set; }                       // Importado pela API.+
            public string codigo_empresa { get; set; }                         // DEPRECATED
            public string codigo_empresa_integracao { get; set; }           // DEPRECATED

        }
        public class Departamento
        {
            public string cCodDepto { get; set; }                   // ID do Departamento.+
            public decimal nPerc { get; set; }                      // Percentual de Rateio.+
            public decimal nValor { get; set; }                     // Valor do Rateio.+
            public string nValorFixo { get; set; }                  // Indica que o valor foi fixado na distribuição do rateio.+

        }
        public class Frete
        {
            public string codigo_transportadora { get; set; }                      // ID da transportadora.+
            public string codigo_transportadora_integracao { get; set; }        // Código Integração da Transportadora.+
            public string modalidade { get; set; }                              // Tipo de Frete.+
            public string placa { get; set; }                                   // Placa do Veículo.+
            public string placa_estado { get; set; }                            // Estado da Placa do Veículo.+
            public string registro_transportador { get; set; }                  // RNTRC (ANTT) - Registro Nacional de Transportador de Cargas.+
            public string quantidade_volumes { get; set; }                         // Quantidade de Volumes.+
            public string especie_volumes { get; set; }                         // Espécie dos Volumes.+
            public string marca_volumes { get; set; }                           // Marca dos Volumes.+
            public string numeracao_volumes { get; set; }                       // Numeração dos Volumes.+
            public decimal peso_liquido { get; set; }                           // Peso Líquido (Kg).+
            public decimal peso_bruto { get; set; }                             // Peso Bruto (Kg).+
            public decimal valor_frete { get; set; }                            // Valor do Frete.+
            public decimal valor_seguro { get; set; }                           // Valor do Seguro.+
            public string numero_lacre { get; set; }                            // Número do Lacre.+
            public decimal outras_despesas { get; set; }                        // Outras Despesas Acessórias.+
            public string veiculo_proprio { get; set; }                         // O transporte será realizado com veículo próprio.+
            public string previsao_entrega { get; set; }                        // Previsão de entrega do Pedido de Venda+
            public string codigo_rastreio { get; set; }                         // Código de Rastreio da Entrega do Pedido de Venda.+
            public IcmsRetido icms_retido { get; set; }                         // Dados do ICMS Retido do Serviço de Transporte.

        }

        public class IcmsRetido
        {
            public decimal vServicoTr { get; set; }                 // Valor de Serviço de Transporte.+
            public decimal vBCRetencaoTr { get; set; }              // Base de Cálculo da Retenção.+
            public decimal vAliquotaRetencaoTr { get; set; }        // Percentual de Alíquota de Retenção.+
            public decimal vIcmsRetidoTr { get; set; }              // Valor de ICMS Retido.+
            public string cCfopTr { get; set; }                     // CFOP.+
            public string cCidadeTr { get; set; }                   // Cidade de Ocorrência do Fato Gerador do ICMS.+
        }

        public class InformacoesAdicionais
        {

            public string codigo_categoria { get; set; }                    // Código da categoria.+
            public string codigo_conta_corrente { get; set; }                  // Código da Conta Corrente.+
            public string numero_pedido_cliente { get; set; }               // Número do pedido do cliente.+
            public string numero_contrato { get; set; }                     // Número do Contrato de Venda.+
            public string contato { get; set; }                             // Contato.+
            public string dados_adicionais_nf { get; set; }                 // Dados adicionais para a Nota Fiscal.+
            public string consumidor_final { get; set; }                    // Nota Fiscal para Consumo Final.+
            public string utilizar_emails { get; set; }                     // Utilizar os seguintes endereços de e-mail.+
            public string enviar_email { get; set; }                        // Enviar e-mail com o boleto de cobrança gerado pelo faturamento (juntamente com o DANFE e o XML da NF-e).+
            public string codVend { get; set; }                                // Código do Vendedor.+
            public string codProj { get; set; }                                // Código do Projeto.+
            public OutrosDetalhes outros_detalhes { get; set; }             // Outros detalhes da NF-e.+
            public string impostos_embutidos { get; set; }                  // Indica se os impostos estão embutidos no valor unitário do item[S / N]
            public string meio_pagamento { get; set; }                      // Meio de Pagamento - Utilizado apenas para emissão de NF-e(campo "tPag" do XML). +
            public string descr_meio_pagamento { get; set; }                // Descrição do Meio de Pagamento - Utilizado apenas para emissão de NF-e(campo "xPag" do XML).+
            public string tipo_documento { get; set; }                      // Tipo de Documento.+
        }
        public class OutrosDetalhes
        {
            public string cNatOperacaoOd { get; set; }              // Natureza da Operação.+
            public string cIndicadorOd { get; set; }                // Indicador de Presença da Operação.+
            public string cIndicadorIntOd { get; set; }             // Indicador de Intermediador.+
            public string cCnpjIntOd { get; set; }                  // CNPJ do Intermediador.+
            public string cIdentificadorIntOd { get; set; }         // Identificação no Intermediador.+
            public string dDataSaidaOd { get; set; }                // Data de Saída.+
            public string cHoraSaidaOd { get; set; }                // Hora de Saída.+
            public string cCnpjCpfOd { get; set; }                  // CNPJ / CPF(do recebedor).+
            public string cNomeOd { get; set; }                     // Nome / Razão Social.+
            public string cInscrEstadualOd { get; set; }            // Inscrição Estadual.+
            public string cEnderecoOd { get; set; }                 // Endereço.+
            public string cNumeroOd { get; set; }                   // Número do endereço.+
            public string cComplementoOd { get; set; }              // Complemento do endereço.+
            public string cBairroOd { get; set; }                   // Bairro.+
            public string cEstadoOd { get; set; }                   // Estado.+
            public string cCidadeOd { get; set; }                   // Cidade.+
            public string cCEPOd { get; set; }                      // CEP.+
            public string cTelefoneOd { get; set; }                 // Telefone.+
        }


        public class ListaParcelas
        {
            public List<Parcela> parcela { get; set; }              // Dados da parcela.
        }
        public class Parcela
        {
            public string numero_parcela { get; set; }                 // Número da Parcela.+
            public decimal valor { get; set; }                      // Valor da Parcela.+
            public decimal percentual { get; set; }                 // Percentual da Parcela.+
            public string data_vencimento { get; set; }             // Data de Vencimento da Parcela.+
            public string quantidade_dias { get; set; }                // Quantidade de dias até o vencimento da parcela.+
            public string tipo_documento { get; set; }              // Tipo de Documento.+
            public string meio_pagamento { get; set; }              // Meio de Pagamento - Utilizado apenas para emissão de NF-e (campo "tPag" do XML). +
            public string descr_meio_pagamento { get; set; }        // Descrição do Meio de Pagamento - Utilizado apenas para emissão de NF-e(campo "xPag" do XML).+
            public string nsu { get; set; }                         // Número Sequencial Único NSU - Para identificar vendas por cartões ou TransactionID TID - Para identificar vendas de comercio eletrônico.+
            public string nao_gerar_boleto { get; set; }            // Não gerar boleto para esta parcela ao emitir a nota fiscal.+
            public string parcela_adiantamento { get; set; }        // Está é uma parcela de Adiantamento do Cliente.+
            public string categoria_adiantamento { get; set; }      // Código da Categoria para o Adiantamento.+
            public string conta_corrente_adiantamento { get; set; }    // Conta Corrente de Adiantamento.+
        }

        public class Observacoes
        {
            public string obs_venda { get; set; }                                   // Observações da venda(elas não serão exibidas na Nota Fiscal).+
        }

        public class Det
        {
            public Det()
            {
                inf_adic = new InfAdic();
            }
            public Ide ide { get; set; }                                            // Identificação do Item do Pedido de Vendas.+
            public Produto produto { get; set; }                                    // Identificação do Produto do Item do Pedido de Vendas.+
            public Observacao observacao { get; set; }                              // Dados da aba 'Observações' do Item do Pedido de Vendas.+
            public InfAdic inf_adic { get; set; }                                   // Dados da aba 'Informações Adicionais' do Item do Pedido de Vendas.+
            public Imposto imposto { get; set; }                                    // Informações referentes aos impostos do Item do Pedido de Vendas.+
            public Rastreabilidade rastreabilidade { get; set; }                    // Dados da rastreabilidade do produto.
            public Combustivel combustivel { get; set; }                            // Detalhamento específico de Combustível.
        }

        public class Ide
        {
            public string codigo_item_integracao { get; set; }                      // Código de Integração do Item do Pedido de Venda.+
            public string codigo_item { get; set; }                                    // ID do Item do Pedido.+
            public string simples_nacional { get; set; }                            // Indica que a empresa é Optante pelo Simples Nacional.+
            public string acao_item { get; set; }                                   // Ação para o item.+
            public string id_ordem_producao { get; set; }                              // Id da Ordem de Produção.+
            public string regra_impostos { get; set; }                                 // DEPRECATED
        }

        public class Produto
        {
            public string codigo_produto { get; set; }                                 // ID do Produto.+
            public string codigo_produto_integracao { get; set; }                   // Código de integração do Produto.+
            public string codigo { get; set; }                                      // Código do Produto exibido na tela do Pedido de Vendas.+
            public string descricao { get; set; }                                   // Descrição do Produto.+
            public string cfop { get; set; }                                        // CFOP - Código Fiscal de Operações e Prestações.+
            public string ncm { get; set; }                                         // NCM - Nomenclatura Comum do Mercosul+
            public string ean { get; set; }                                         // EAN - European Article Number+
            public string unidade { get; set; }                                     // Unidade.+
            public decimal quantidade { get; set; }                                 // Quantidade+
            public decimal valor_unitario { get; set; }                             // Valor Únitário+
            public string codigo_tabela_preco { get; set; }                            // Código da tabela de preço.+
            public decimal valor_mercadoria { get; set; }                           // Valor da Mercadoria+
            public string tipo_desconto { get; set; }                               // Tipo de Desconto.+
            public decimal percentual_desconto { get; set; }                        // Percentual de Desconto.+
            public decimal valor_desconto { get; set; }                             // Valor do Desconto+
            public decimal valor_deducao { get; set; }                              // Valor da Dedução+
            public decimal valor_icms_desonerado { get; set; }                      // Valor do ICMS desonerado
            public string motivo_icms_desonerado { get; set; }                      // Código do Motivo de desoneração do ICMS+
            public decimal valor_total { get; set; }                                // Valor Total.+
            public string indicador_escala { get; set; }                            // Indicador de Produção em Escala Relevante.+
            public string cnpj_fabricante { get; set; }                             // CNPJ do Fabricante da Mercadoria.
        }

        public class Observacao
        {
            public string obs_venda { get; set; }                                   // Observações da venda(elas não serão exibidas na Nota Fiscal).
        }

        public class InfAdic
        {
            public decimal peso_liquido { get; set; }                               // Peso Líquido(Kg).+
            public decimal peso_bruto { get; set; }                                 // Peso Bruto(Kg).+
            public string numero_pedido_compra { get; set; }                        // Numero do Pedido de Compra.+
            public string item_pedido_compra { get; set; }                             // Item do Pedido de Compra.+
            public string dados_adicionais_item { get; set; }                       // Informações para a Nota Fiscal.+
            public string nao_movimentar_estoque { get; set; }                      // Não gerar a saída de estoque deste item ao emitir NF-e.+
            public string nao_gerar_financeiro { get; set; }                        // Não gerar conta a receber para este item.+
            public string codigo_categoria_item { get; set; }                       // Código da Categoria do item.+
            public string codigo_beneficio_fiscal { get; set; }                     // Código do Benefício Fiscal.
            public string numero_fci { get; set; }                                  // Número da FCI (Ficha de Conteúdo de Importação)
            public string codigo_local_estoque { get; set; }                           // Codigo do Local do Estoque.+
            public string codigo_cenario_impostos_item { get; set; }                   // Código do Cenário de Impostos.+
        }

        public class Imposto
        {
            public IcmsSn icms_sn { get; set; }                     // icms_sn ICMS - Simples Nacional.+
            public Icms icms { get; set; }                          // icms    ICMS+
            public IcmsSt icms_st { get; set; }                     // icms_st ICMS - Substituição Tributária.+
            public IcmsIe icms_ie { get; set; }                     // icms_ie ICMS Interestadual.+
            public Ipi ipi { get; set; }                            // ipi IPI.+
            public PisPadrao pis_padrao { get; set; }               // pis_padrao  PIS.+
            public PisSt pis_st { get; set; }                       // pis_st  PIS - Substituíção Tributária.+
            public CofinsPadrao cofins_padrao { get; set; }         // cofins_padrao   COFINS.+
            public CofinsSt cofins_st { get; set; }                 // cofins_st   COFINS - Substituição Tributária.+
            public Inss inss { get; set; }                          // inss    INSS.+
            public Csll csll { get; set; }                          // csll    CSLL.+
            public Irrf irrf { get; set; }                          // irrf    IRRF.+
            public Iss iss { get; set; }                            // iss ISS.+
        }

        public class IcmsSn
        {
            public string cod_sit_trib_icms_sn { get; set; }                    // Código da situação tributária pelo Simples
            public string origem_icms_sn { get; set; }                          // NFe - Origem
            public decimal aliq_icms_sn { get; set; }                           // Alíquota aplicável de cálculo do crédito de ICMS no Simples Nacional
            public decimal valor_credito_icms_sn { get; set; }                  // Valor do crédito de ICMS no Simples Nacional
            public decimal base_icms_sn { get; set; }                           // Base de Cálculo da UF Remetente.+
            public decimal valor_icms_sn { get; set; }                          // Valor do ICMS retido anteriormente por Substituição Tributária
        }

        public class Icms
        {
            public string cod_sit_trib_icms { get; set; }                   // NFe - Situação Tributária
            public string origem_icms { get; set; }                         // NFe - Origem
            public string modalidade_icms { get; set; }                     // NFe - Modalidade para determinação da Base de Cálculo do ICMs
            public decimal perc_red_base_icms { get; set; }                 // Percentual de redução da base de cálculo do ICMS
            public decimal base_icms { get; set; }                          // Base de cálculo do ICMS
            public decimal aliq_icms { get; set; }                          // Alíquota do ICMS
            public decimal valor_icms { get; set; }                         // Valor do ICMS
            public decimal perc_fcp_icms { get; set; }                      // Percentual do FCP - Fundo de Combate a Pobreza do ICMS.
            public decimal base_fcp_icms { get; set; }                      // Base de Cálculo do FCP - Fundo de Combate a Pobreza do ICMS.
            public decimal valor_fcp_icms { get; set; }                     // Valor do FCP - Fundo de Combate a Pobreza do ICMS.
        }

        public class IcmsSt
        {
            public string cod_sit_trib_icms_st { get; set; }                // string2 NFe - Situação Tributária
            public string modalidade_icms_st { get; set; }                  // string1 NFe - Código da Modalidade de determinação da Base de Cálculo do ICMS ST
            public decimal perc_red_base_icms_st { get; set; }              // decimal Percentual de redução da base de cálculo do ICMS
            public decimal base_icms_st { get; set; }                       // decimal Base de cálculo do ICMS Substituição Tributária
            public decimal aliq_icms_st { get; set; }                       // decimal Alíquota do ICMS Substituição Tributária
            public decimal margem_icms_st { get; set; }                     // decimal Percentual da margem do valor adicionado da base de cálculo do ICMS ST
            public decimal valor_icms_st { get; set; }                      // decimal Valor do ICMS Substituição Tributária
            public decimal aliq_icms_opprop { get; set; }                   // decimal Alíquota de ICMS Operação Própria.
            public string cest { get; set; }                                // string9 CEST - Código Especificador da Substituíção Tributária.+
            public decimal perc_fcp_icms_st { get; set; }                   // decimal Percentual do FCP - Fundo de Combate a Pobreza do ICMS ST.
            public decimal base_fcp_icms_st { get; set; }                   // decimal Base de Cálculo do FCP - Fundo de Combate a Pobreza do ICMS ST.
            public decimal valor_fcp_icms_st { get; set; }                  // decimal Valor do FCP - Fundo de Combate a Pobreza do ICMS ST.
        }

        public class IcmsIe
        {
            public decimal base_icms_uf_destino { get; set; }               // BC do ICMS na UF de Destino
            public decimal aliq_icms_FCP { get; set; }                      // Percentual do ICMS relativo ao Fundo de Combate à Pobreza(FCP) na UF de Destino
            public decimal aliq_interna_uf_destino { get; set; }            // Alíquota Interna da UF de Destino
            public decimal aliq_interestadual { get; set; }                 // Alíquota Interestadual das UFs Envolvidas
            public decimal aliq_partilha_icms { get; set; }                 // Percentual Provisório de Partilha do ICMS Interestadual
            public decimal valor_fcp_icms_inter { get; set; }               // Valor do fundo de combate a pobreza.
            public decimal valor_icms_uf_dest { get; set; }                 // Valor do ICMS - UF Destino
            public decimal valor_icms_uf_remet { get; set; }                // Valor do ICMS - UF Remetente
        }

        public class Ipi
        {
            public string cod_sit_trib_ipi { get; set; }                    // Código da situação tributária do IPI
            public string tipo_calculo_ipi { get; set; }                    // Tipo de cálculo para obtenção do valor do IPI
            public string enquadramento_ipi { get; set; }                   // Enquadramento do IPI
            public decimal base_ipi { get; set; }                           // Base de Cálculo do IPI
            public decimal aliq_ipi { get; set; }                           // Alíquota do IPI
            public decimal qtde_unid_trib_ipi { get; set; }                 // Quantidade de Unidades Tributáveis do IPI
            public decimal valor_unid_trib_ipi { get; set; }                // Valor do IPI por Unidade Tributável
            public decimal valor_ipi { get; set; }                          // Valor do IPI
        }

        public class PisPadrao
        {
            public string cod_sit_trib_pis { get; set; }                    // Código da Situação Tributária do PIS
            public string tipo_calculo_pis { get; set; }                    // Tipo de cálculo para obtenção do valor do PIS
            public decimal base_pis { get; set; }                           // Base de Cálculo do PIS
            public decimal aliq_pis { get; set; }                           // Alíquota do PIS
            public decimal qtde_unid_trib_pis { get; set; }                 // Quantidade de Unidades Tributáveis do PIS
            public decimal valor_unid_trib_pis { get; set; }                // Valor do PIS por Unidade Tributável
            public decimal valor_pis { get; set; }                          // Valor do PIS
        }

        public class PisSt
        {
            public string cod_sit_trib_pis_st { get; set; }                 // Código da Situação Tributária do PIS
            public string tipo_calculo_pis_st { get; set; }                 // Tipo de cálculo para obtenção do valor do PIS Substituição Tributária
            public decimal base_pis_st { get; set; }                        // Base de Cálculo do PIS Substituição Tributária
            public decimal aliq_pis_st { get; set; }                        // Alíquota do PIS Substituição Tributária
            public decimal qtde_unid_trib_pis_st { get; set; }              // Quantidade de Unidades Tributáveis do PIS Substituição Tributária
            public decimal valor_unid_trib_pis_st { get; set; }             // Valor do PIS Substituição Tributária por Unidade Tributável
            public decimal margem_pis_st { get; set; }                      // Margem de valor adicional para obter a base de cálculo do PIS Substituição Tributária
            public decimal valor_pis_st { get; set; }                       // Valor do PIS Substituição Tributária
        }

        public class CofinsPadrao
        {
            public string cod_sit_trib_cofins { get; set; }                 // Código da Situação Tributária do COFINS
            public string tipo_calculo_cofins { get; set; }                 // Tipo de cálculo para obtenção do valor do PIS
            public decimal base_cofins { get; set; }                        // Base de Cálculo do COFINS
            public decimal aliq_cofins { get; set; }                        // Alíquota do COFINS
            public decimal qtde_unid_trib_cofins { get; set; }              // Quantidade de Unidades Tributáveis do COFINS
            public decimal valor_unid_trib_cofins { get; set; }             // Valor do COFINS por Unidade Tributável
            public decimal valor_cofins { get; set; }                       // Valor do COFINS
        }

        public class CofinsSt
        {
            public string cod_sit_trib_cofins_st { get; set; }              // Código da Situação Tributária do COFINS
            public string tipo_calculo_cofins_st { get; set; }              // Tipo de cálculo para obtenção do valor do PIS Substituição Tributária
            public decimal base_cofins_st { get; set; }                     // Base de Cálculo do COFINS Substituição Tributária
            public decimal aliq_cofins_st { get; set; }                     // Alíquota do COFINS Substituição Tributária
            public decimal qtde_unid_trib_cofins_st { get; set; }           // Quantidade de Unidades Tributáveis do PIS Substituição Tributária
            public decimal valor_unid_trib_cofins_st { get; set; }          // Valor do PIS Substituição Tributária por Unidade Tributável
            public decimal margem_cofins_st { get; set; }                   // Margem de valor adicional para obter a base de cálculo do COFINS Substituição Tributária
            public decimal valor_cofins_st { get; set; }                    // Valor do PIS Substituição Tributária
        }

        public class Inss
        {
            public decimal aliq_inss { get; set; }                          // Alíquota do INSS
            public decimal valor_inss { get; set; }                         // Valor do INSS
        }

        public class Csll
        {
            public decimal aliq_csll { get; set; }                          // Alíquota do CSLL
            public decimal valor_csll { get; set; }                         // Valor do CSLL
        }

        public class Irrf
        {
            public decimal aliq_irrf { get; set; }                          // Alíquota do IRRF
            public decimal valor_irrf { get; set; }                         // Valor do IRRF
        }

        public class Iss
        {
            public decimal base_iss { get; set; }                           // Base de cálculo do ISS
            public decimal aliq_iss { get; set; }                           // Alíquota do ISS
            public decimal valor_iss { get; set; }                          // Valor do ISS
            public string retem_iss { get; set; }                           // Indica que o valor do ISS será retido pelo tomador do serviço
        }

        public class Rastreabilidade
        {
            public string numeroLote { get; set; }                          // Número do Lote
            public decimal qtdeProdutoLote { get; set; }                    // Quantidade de Produto
            public string dataFabricacaoLote { get; set; }                  // Data de Fabricação/Produção
            public string dataValidadeLote { get; set; }                    // Data de Validade
            public string codigoAgregacaoLote { get; set; }                 // Código de Agregação
        }

        public class Combustivel
        {
            public string cCodigoANP { get; set; }                          // Código de Produto da ANP.
            public string cDescrANP { get; set; }                           // Descrição do Produto conforme ANP.
            public string cCODIF { get; set; }                              // Registro do CODIF.
            public decimal nPercGLP { get; set; }                           // Percentual de GLP Derivado do Petróleo.
            public decimal nPercGasNacional { get; set; }                   // Percentual de Gás Natural Nacional.
            public decimal nPercGasImportado { get; set; }                  // Percentual de Gás Natural Importado.
            public decimal nValorPartida { get; set; }                      // Valor de Partida.
            public decimal nQtdeFatTempAmb { get; set; }                    // Quantidade Faturada à Temperatura Ambiente.
            public decimal cUFConsumoComb { get; set; }                     // UF de Consumo.
            public decimal nBC_CIDE { get; set; }                           // Base de Cálculo (em quantidade) da CIDE.
            public decimal nAliquota_CIDE { get; set; }                     // Valor da Alíquota em Reais da CIDE.
            public decimal nValor_CIDE { get; set; }                        // Valor da CIDE.
            public decimal nBC_UFRem { get; set; }                          // Base de Cálculo da UF Remetente.+
            public decimal nValorUFRem { get; set; }                        // Valor da UF Remetente.+
            public decimal nBC_UFDest { get; set; }                         // Base de Cálculo da UF Destino.+
            public decimal nValorUFDest { get; set; }                       // Valor da UF Destino.+
        }

        public class MarketPlace
        {
            public decimal nTaxa { get; set; }                              // Valor da taxa do Market Place.
            public decimal nEnvio { get; set; }                             // Valor do envio do Market Place.
        }

        public class TotalPedido
        {
            public decimal base_calculo_icms { get; set; }                  // Base de Cálculo do ICMS.+
            public decimal base_calculo_st { get; set; }                    // Base de cálculo da substituição tributária.+
            public decimal valor_pis { get; set; }                          // Valor do PIS.+
            public decimal valor_cofins { get; set; }                       // Valor do cofins.+
            public decimal valor_csll { get; set; }                         // Valor da CSLL.+
            public decimal valor_icms { get; set; }                         // Valor total do ICMS.+
            public decimal valor_st { get; set; }                           // Valor total da ST.+
            public decimal valor_inss { get; set; }                         // Valor do INSS.+
            public decimal valor_IPI { get; set; }                          // Valor do IPI.+
            public decimal valor_ir { get; set; }                           // Valor do IR.+
            public decimal valor_iss { get; set; }                          // Valor do ISS.+
            public decimal valor_deducoes { get; set; }                     // Valor das deduções.+
            public decimal valor_descontos { get; set; }                    // Valor dos descontos.+
            public decimal valor_mercadorias { get; set; }                  // valor das mercadorias.+
            public decimal valor_total_pedido { get; set; }                 // Valor total do pedido.+
        }

        public class InfoCadastro
        {
            public string dInc { get; set; }                        // Data da Inclusão.+
            public string hInc { get; set; }                        // Hora da Inclusão.+
            public string uInc { get; set; }                        // Usuário da Inclusão.+
            public string dAlt { get; set; }                        // Data da Alteração.+
            public string hAlt { get; set; }                        // Hora da Alteração.+
            public string uAlt { get; set; }                        // Usuário da Alteração.+
            public string dCan { get; set; }                        // Data da Cancelamento.+
            public string hCan { get; set; }                        // Hora da Cancelamento.+
            public string uCan { get; set; }                        // Usuário da Cancelamento.+
            public string cancelado { get; set; }                   // Indica se o pedido foi cancelado.+
            public string dFat { get; set; }                        // Data de Faturamento.+
            public string hFat { get; set; }                        // Hora de faturamento.+
            public string uFat { get; set; }                        // Usuário que realizou o faturamento.+
            public string faturado { get; set; }                    // Indica se o pedido está faturado.+
            public string denegado { get; set; }                    // Indica se o pedido foi denegado.+
            public string autorizado { get; set; }                  // Indica se o pedido foi autorizado
            public string cImpAPI { get; set; }                     // Importado pela API.+
        }

        public class Exportacao
        {
            public string nao_exportacao { get; set; }              // O cliente é estrangeiro, mas na verdade não se trata de uma exportação
            public string local_embarque { get; set; }              // Local de Embarque(ou de transposição de fronteira)
            public string uf_embarque { get; set; }                 // UF do Local de Embarque
            public string local_despacho { get; set; }              // Local de Despacho
        }
    }
}