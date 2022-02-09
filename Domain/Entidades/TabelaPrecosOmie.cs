using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class TabelaPrecosOmie
    {

        public TabelaPrecosOmie()
        {
            itensTabela = new List<ItensTabela>();
        }
        public string nCodTabPreco { get; set; }                    // Código da Tabela de Preços.+
        public string cCodIntTabPreco { get; set; }                 // Código de integração da Tabela de Preços.+
        public string cNome { get; set; }                           // Nome da Tabela de Preço.
        public string cCodigo { get; set; }                         // Código da Tabela de Preço.+
        public string cAtiva { get; set; }                          // Indica se a tabela de preços está ativa(S/N).+
        public string cOrigem { get; set; }                         // Origem da Tabela de Preços.+
        public List<ItensTabela> itensTabela { get; set; }               // Itens da Tabela de Preços.
        public Clientes clientes { get; set; }


        public const string VersaoAPI = "v1/produtos/";
        public const string Endpoint = "tabelaprecos/";
        public const string UrlApi = "https://app.omie.com.br/api/v1/produtos/tabelaprecos/";

    }
    public class Clientes
    {
        public string CTag { get; set; }
        public string CTodosClientes { get; set; }
        public string CUF { get; set; }
        public int NCodTag { get; set; }
    }
    public class ItensTabela
    {

        public long nCodProd { get; set; }                                   // Código do produto.+
        public string cCodIntProd { get; set; }                             // Código de integração do produto.+
        public string cCodigoProduto { get; set; }                          // Código do produto.+
        public string cDescricaoProduto { get; set; }                       // Descrição do produto.
        public string cNCM { get; set; }                                    // Código do NCM.+
        public string cEAN { get; set; }                                    // EAN do produto.
        public long nCodFamilia { get; set; }                                // Código da Familia do Produto.+
        public string cArredPreco { get; set; }                             // Indica se deve Arredondar o preço do produto(S/N).
        public string cTemDesconto { get; set; }                            // Indica se a Tabela de Preço possui Desconto Sugerido ou Máximo(S/N).
        public decimal nDescMaximo { get; set; }                            // Percentual de Desconto Máximo Permitido.
        public decimal nDescSugerido { get; set; }                          // Percentual de Desconto Sugerido.
        public string cManual { get; set; }                                 // Indica se o item foi modificado Manualmente (S/N).
        public decimal nPercDesconto { get; set; }                          // Percentual de Desconto da tabela de preços.
        public decimal nPercAcrescimo { get; set; }                         // Percentual de Acréscimo da tabela de preços.
        public decimal nValorCalculado { get; set; }                        // Valor Calculado.
        public decimal nValorOriginal { get; set; }                         // Valor Original.
        public decimal nValorTabela { get; set; }                           // Valor da Tabela de Preços.
        public ItensInfo itemInfo { get; set; }                             // Informações do cadastro do item.
    }
    public class ItensInfo
    {

        public string dIncItem { get; set; }                    // Data da Inclusão.+
        public string hIncItem { get; set; }                    // Hora da Inclusão.+
        public string uIncItem { get; set; }                    // Usuário da Inclusão.
        public string dAltItem { get; set; }                    // Data da Alteração.+
        public string hAltItem { get; set; }                    // Hora da Alteração.+
        public string uAltItem { get; set; }                    // Usuário da Alteração.
        public string cImpAPIItem { get; set; }                 // Importado pela API(S/N).
    }


}
