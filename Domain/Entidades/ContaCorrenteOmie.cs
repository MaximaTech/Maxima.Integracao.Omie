namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class ContaCorrenteOmie
    {
        public const string VersaoAPI = "v1/";
        public const string Endpoint = "geral/contacorrente/";
        public const string UrlApi = "https://app.omie.com.br/api/v1/geral/contacorrente/";


        public long nCodCC { get; set; }                             // Código da conta corrente no Omie.
        public string cCodCCInt { get; set; }                       // Código de Integração do Parceiro.
        public string tipo_conta_corrente { get; set; }             // Tipo da Conta Corrente.+
        public string codigo_banco { get; set; }                    // Código do banco.+
        public string descricao { get; set; }                       // Descrição da conta corrente.
        public string codigo_agencia { get; set; }                  // Código da Agência
        public string numero_conta_corrente { get; set; }           // Número da conta corrente.
        public decimal saldo_inicial { get; set; }                  // Saldo Inicial da Conta Corrente
        public string saldo_data { get; set; }                      // Data do Saldo Inicial da Conta Corrente
        public decimal valor_limite { get; set; }                   // Valor do Limite do Crédito
        public string nao_fluxo { get; set; }                       // Não exibir no Fluxo de Caixa
        public string nao_resumo { get; set; }                      // Não exibir no Resumo de Finanças
        public string observacao { get; set; }                      // Observação
        public string cobr_sn { get; set; }                         // Indica se realiza Cobrança Bancária para a conta corrente[S / N]
        public decimal per_juros { get; set; }                      // Percentual de Juros ao Mês
        public decimal per_multa { get; set; }                      // Percentual de Multa
        public string bol_instr1 { get; set; }                      // Mensagem de Instrução do Boleto - Linha 1
        public string bol_instr2 { get; set; }                      // Mensagem de Instrução do Boleto - Linha 2
        public string bol_instr3 { get; set; }                      // Mensagem de Instrução do Boleto - Linha 3
        public string bol_instr4 { get; set; }                      // Mensagem de Instrução do Boleto - Linha 4
        public string bol_sn { get; set; }                          // Indica se emite Boletos de Cobrança [S/N]
        public string cnab_esp { get; set; }                        // Espécie padrão para a Remessa de Cobrança
        public string cobr_esp { get; set; }                        // Espécie padrão para o Boleto de Cobrança
        public string dias_rcomp { get; set; }                      // Dias para Compensação dos Recebimentos
        public string modalidade { get; set; }                      // Modalidade da Cobrança
        public string cancinstr { get; set; }                       // Código de Instrução de Cancelamento, Baixa ou Devolução
        public string pdv_enviar { get; set; }                      // Utiliza a Conta Corrente no OmiePDV
        public string pdv_sincr_analitica { get; set; }             // Sincroniza os Movimentos de Forma Análitica para o PDV
        public int pdv_dias_venc { get; set; }                      // Dias para vencimento
        public int pdv_num_parcelas { get; set; }                   // Número máximo de parcelas do Cartão de Credito
        public int pdv_tipo_tef { get; set; }                       // Tipo de TEF
        public int pdv_cod_adm { get; set; }                        // Código da Administradora de Cartões
        public int pdv_limite_pacelas { get; set; }                 // Limite máximo de parcelas
        public decimal pdv_taxa_loja { get; set; }                  // Taxa de Juros da Loja
        public decimal pdv_taxa_adm { get; set; }                   // Taxa da Administradora de Cartões
        public string pdv_categoria { get; set; }                   // Código da Categoria para o PDV.
        public string cTipoCartao { get; set; }                     // Tipo de Cartão para Administradoras de Cartão.+
        public string nome_gerente { get; set; }                    // Nome do Gerente da Conta Corrente.
        public string ddd { get; set; }                             // DDD do Telefone de Contato do Gerente da Agência
        public string telefone { get; set; }                        // Telefone de Contato do Gerente da Agência
        public string email { get; set; }                           // E-mail do Gerente da Conta Corrente
        public string endereco { get; set; }                        // Endereço da Agência
        public string numero { get; set; }                          // Número do Endereço
        public string bairro { get; set; }                          // Bairro
        public string complemento { get; set; }                     // Complemento do Número do Endereço
        public string estado { get; set; }                          // Estado da Agência
        public string cidade { get; set; }                          // Cidade da Agência
        public string cep { get; set; }                             // CEP da Agência
        public string codigo_pais { get; set; }                     // Código do País
        public string data_inc { get; set; }                        // Data de Inclusão
        public string hora_inc { get; set; }                        // Hora de Inclusão
        public string user_inc { get; set; }                        // Usuário da Inclusão
        public string data_alt { get; set; }                        // Data de alteração
        public string hora_alt { get; set; }                        // Hora de Alteração
        public string user_alt { get; set; }                        // Usuário de Alteração
        public string importado_api { get; set; }                   // Registro importado pela API
        public string bloqueado { get; set; }                       // Registro Bloqueado pela API
        public string inativo { get; set; }                         // Indica se o cadastro da conta corrente está inativo (S/N).
    }
}
