using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class PesquisaTituloOmie
    {
        public CabecTitulo cabecTitulo { get; set; }        // Informações sobre o título encontrado
        public List<Lancamentos> lancamentos { get; set; }        // Lançamentos realizados para o título
        public Resumo resumo { get; set; }                  // Resumo dos pagamentos realizados para o título

        public const string VersaoAPI = "v1/financas/";

        public const string Endpoint = "pesquisartitulos/";

        public const string UrlApi = "https://app.omie.com.br/api/v1/financas/pesquisartitulos/";

        public class CabecTitulo
        {
            public long nCodTitulo { get; set; }                             // Código do título.+
            public string cCodIntTitulo { get; set; }                       // Código de integração do título.+
            public string cNumTitulo { get; set; }                          // Número do título.+
            public string dDtEmissao { get; set; }                          // Data de emissão do título.
            public string dDtVenc { get; set; }                             // Data de emissão do título.
            public string dDtPrevisao { get; set; }                         // Data de previsão de Pagamento/Recebimento.
            public string dDtPagamento { get; set; }                        // Data de pagamento do título.
            public string nCodCliente { get; set; }                            // Código de Cliente / Fornecedor.+
            public string cCPFCNPJCliente { get; set; }                     // Filtrar os títulos por CPF/CNPJ do cliente.
            public string nCodCtr { get; set; }                                // Código do contrato associado ao título.+
            public string cNumCtr { get; set; }                             // Número do contrato associado ao título.+
            public string nCodOS { get; set; }                                 // Código do Pedido de Venda / Ordem de Serviço.+
            public string cNumOS { get; set; }                              // Número do pedido de venda / Ordem de Serviço.+
            public string nCodCC { get; set; }                                 // Código da conta corrente.+
            public string cStatus { get; set; }                             // Status do título.+
            public string cNatureza { get; set; }                           // Natureza do título.+
            public string cTipo { get; set; }                               // Tipo de documento.+
            public string cOperacao { get; set; }                           // Operação do título.+
            public string cNumDocFiscal { get; set; }                       // Número do documento fiscal (NF-e, NFC-e, NFS-e, etc)+
            public string cCodCateg { get; set; }                           // Código da categoria.
            public List<aCodCateg> aCodCateg { get; set; }                   // Rateio de categorias
            public string cNumParcela { get; set; }                         // Número da parcela "Formatada" como 999/999
            public decimal nValorTitulo { get; set; }                       // Valor do título.
            public decimal nValorPIS { get; set; }                          // Valor do PIS.
            public string cRetPIS { get; set; }                             // Indica que o Valor do PIS informado deve ser retido.
            public decimal nValorCOFINS { get; set; }                       // Valor do COFINS.
            public string cRetCOFINS { get; set; }                          // Indica que o Valor do COFINS informado deve ser retido.
            public decimal nValorCSLL { get; set; }                         // Valor do CSLL.
            public string cRetCSLL { get; set; }                            // Indica que o Valor do CSLL informado deve ser retido.
            public decimal nValorIR { get; set; }                           // Valor do Imposto de Renda.
            public string cRetIR { get; set; }                              // Indica que o Valor do Imposto de Renda informado deve ser retido.
            public decimal nValorISS { get; set; }                          // Valor do ISS.
            public string cRetISS { get; set; }                             // Indica que o Valor do ISS informado deve ser retido.
            public decimal nValorINSS { get; set; }                         // Valor do INSS.
            public string cRetINSS { get; set; }                            // Indica que o Valor do INSS informado deve ser retido.
            public string observacao { get; set; }                          // Observações do título.
            public string cCodProjeto { get; set; }                            // Código do projeto.+
            public string cCodVendedor { get; set; }                           // Código do vendedor.+
            public string nCodComprador { get; set; }                          // Código do comprador.+
            public string cCodigoBarras { get; set; }                       // Código de Barras do título.
            public string cNSU { get; set; }                                // Número Sequencial Único - Comprovante de pagamento.
            public string nCodNF { get; set; }                                 // Código da Nota Fiscal.
            public string dDtRegistro { get; set; }                         // Data de registro da NF.
            public string cNumBoleto { get; set; }                          // Número do Boleto.
            public string cChaveNFe { get; set; }                           // Chave da NF-e, CT-e, NFC-e de origem.
            public string cOrigem { get; set; }                             // Origem do lançamento.+
            public string nCodTitRepet { get; set; }                           // Código do título original que gerou a repetição dos lançamentos.
            public string dDtCanc { get; set; }                             // Data de Cancelamento do título.
            public List<Departamento> departamentos { get; set; }           // Distribuição por departamentos.
        }

        public class Lancamentos
        {
            public string nCodLanc { get; set; }                       // Código do lançamento
            public string cCodIntLanc { get; set; }                 // Código de Integração do Lançamento
            public string nIdLancCC { get; set; }                      // Código do Lançamento na Conta Corrente
            public string dDtLanc { get; set; }                     // Data do Lançamento
            public decimal nValLanc { get; set; }                   // Valor do Lançamento
            public decimal nMulta { get; set; }                     // Valor da Multa.
            public decimal nDesconto { get; set; }                  // Valor do Desconto.
            public decimal nJuros { get; set; }                     // Valor do Juros.
            public string nCodCC { get; set; }                         // Código da Conta Corrente
            public string cNatureza { get; set; }                   // Natureza do Lançamento.+
            public string cObsLanc { get; set; }                    // Observação para o lançamento}
        }

        public class aCodCateg
        {
            public string cCodCateg { get; set; }           // Código da categoria
            public decimal nValor { get; set; }             // Valor da categoria
            public decimal nPerc { get; set; }              // Percentual da categoria
        }
        public class Departamento
        {
            public string cCodDepartamento { get; set; }        // Código do Departamento.
            public decimal nDistrPercentual { get; set; }       // Percentual da distribuição.
            public decimal nDistrValor { get; set; }            // Valor da distribuição.
            public string nValorFixo { get; set; }              // Indica que o valor foi fixado na distribuição (S/N).
        }

        public class Resumo
        {
            public string cLiquidado { get; set; }                   // Indica se o título está liquidado.+
            public decimal nValPago { get; set; }                    // Valor total pago para o título.
            public decimal nValAberto { get; set; }                  // Valor total em aberto para o título.
            public decimal nDesconto { get; set; }                   // Valor do Desconto.
            public decimal nJuros { get; set; }                      // Valor do Juros.
            public decimal nMulta { get; set; }                      // Valor da Multa.
            public decimal nValLiquido { get; set; }                 // Valor líquido+
        }
    }
}
