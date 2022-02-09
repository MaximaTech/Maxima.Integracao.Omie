namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class CategoriaOmie
    {
        public string codigo { get; set; } // Código para a Categoria
        public string descricao { get; set; } // Descrição para a Categoria
        public string descricao_padrao { get; set; } // Descrição Padrão para a Categoria
        public string conta_inativa { get; set; } // Indica que a conta está inativo
        public string definida_pelo_usuario { get; set; } // Indica que a conta financeira é definida pelo usuário
        public string id_conta_contabil { get; set; } // ID da Conta Contábil
        public string tag_conta_contabil { get; set; } // Tag para Conta Contábil
        public string conta_despesa { get; set; } // Quando S, indica que é conta de despesa
        public string nao_exibir { get; set; } // Indica que a Categoria não deve ser exibida em ComboBox
        public string natureza { get; set; } // Descrição da Natureza da conta
        public string conta_receita { get; set; } // Quando S, indica que é conta de receita
        public string totalizadora { get; set; } // Quando S, indica que é totalizadora de conta
        public string transferencia { get; set; } // Quando S, indica que é categoria de transferência
        public string codigo_dre { get; set; } // Código no DRE
        public string categoria_superior { get; set; } // Código da Categoria de Nivel Superior
        public DadosDREOmie dadosDRE { get; set; }  // Detalhes da conta do DRE.




        public const string VersaoAPI = "v1/geral/";

        public const string Endpoint = "categorias/";
        public const string UrlApi = "https://app.omie.com.br/api/v1/geral/categorias/";

    }
}
