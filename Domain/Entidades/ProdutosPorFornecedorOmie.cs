using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class ProdutosPorFornecedorOmie
    {
        public long nCodForn { get; set; }                           // Código do fornecedor.+
        public string cCodIntForn { get; set; }                     // Código de Integração do fornecedor.+
        public string cCpfCnpj { get; set; }                        // CPF / CNPJ do fornecedor.
        public string cRazaoSocial { get; set; }                    // Razão Social do fornecedor.
        public string cNomeFantasia { get; set; }                   // Nome Fantasia do fornecedor.
        public List<Produtos> produtos { get; set; }                        // Lista de produtos

        public const string VersaoAPI = "v1/estoque/";

        public const string Endpoint = "produtofornecedor/";

        public const string UrlApi = "https://app.omie.com.br/api/v1/estoque/produtofornecedor/";


        public class Produtos
        {
            public long nCodProd { get; set; }                       // Código do produto.+
            public string cCodIntProd { get; set; }                 // Código de Integração do produto.+
            public string cCodigo { get; set; }                     // Código do produto, conforme o fornecedor utiliza.
            public string cDescricao { get; set; }                  // Descrição do produto.
        }
    }
}
