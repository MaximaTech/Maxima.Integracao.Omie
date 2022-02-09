using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Entidades
{
    public class ContaOmie
    {
        public Identificacao identificacao { get; set; }                    // Identificação da conta
        public Endereco endereco { get; set; }                              // Endereço da conta
        public TelefoneEmail telefone_email { get; set; }                   // Informações de contato com a conta
        public InformacoesAdicionais informacoesAdicionais { get; set; }    // Informações adicionais da conta
        public List<Tags> tags { get; set; }                                // Tags da conta+
        public List<Caracteristicas> caracteristicas { get; set; }          //Caracteristicas da conta+


        public class Identificacao
        {
            public int nCod { get; set; }          // Código da conta.
            public string cCodInt { get; set; }    // Código de Integração
            public string cNome { get; set; }      // Nome da Conta
            public string cDoc { get; set; }       // Documento CNPJ / CPF.
            public int nCodVend { get; set; }      // Código do Vendedor
            public int nCodVert { get; set; }      // Código da Vertical
            public int nCodTelemkt { get; set; }   // Código do Telemarketing
            public int dDtReg { get; set; }        // Data de Registro
            public string dDtValid { get; set; }   // Data de Validade da Reserva
            public string cObs { get; set; }       // Observação
        }

        public class Endereco
        {
            public string cEndereco { get; set; }   //  Endereço
            public string cCompl { get; set; }      //  Complemento do Endereço
            public string cCEP { get; set; }        //  CEP
            public string cBairro { get; set; }     //  Bairro
            public string cCidade { get; set; }     //  Cidade
            public string cUF { get; set; }         //  Estado
            public string cPais { get; set; }       //  País
        }
        public class TelefoneEmail
        {
            public string cDDDTel { get; set; }     // DDD do Telefone
            public string cNumTel { get; set; }     // Número do Telefone
            public string cDDDFax { get; set; }     // DDD do Fax
            public string cNumFax { get; set; }     // Número do Fax
            public string cEmail { get; set; }      // E-Mail da conta.
            public string cWebsite { get; set; }    // Website
        }

        public class InformacoesAdicionais
        {

            public int nNumFunc { get; set; }        // Número de Funcionários
            public string nFaixaFat { get; set; }    // Faixa de Faturamento
            public string cCnae { get; set; }        // CNAE Principal
            public string cRegTrib { get; set; }     // Regime Tributário
        }
        public class Tags
        {
            public string tag { get; set; }          // Tag da conta
        }

        public class Caracteristicas
        {
            public string campo { get; set; }        // Nome da característica.
            public string conteudo { get; set; }     // Conteúdo do característica.
        }
    }
}
