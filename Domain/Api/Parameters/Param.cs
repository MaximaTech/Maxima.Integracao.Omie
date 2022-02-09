using Newtonsoft.Json;
using System.Collections.Generic;
using static Maxima.Cliente.Omie.Domain.Entidades.ContaOmie;

namespace Maxima.Cliente.Omie.Domain.Api.Parameters
{
    public class Param
    {
        [JsonProperty("pagina")]
        public long Pagina { get; set; }

        [JsonProperty("registros_por_pagina")]
        public long RegistrosPorPagina { get; set; } = 100;

        [JsonProperty("apenas_importado_api")]
        public string ApenasImportadoApi { get; set; } = "N";

        [JsonProperty("cExibeTodos")]
        public string ExibeTodos { get; set; }

        [JsonProperty("filtrar_apenas_omiepdv")]
        public string FiltrarApenasOmie { get; set; }

        [JsonProperty("inativo")]
        public string Inativo { get; set; }

        [JsonProperty("ordem_decrescente")]
        public string OrdemDecrescente { get; set; }

        [JsonProperty("clientesFiltro")]
        public ClientesFiltros ClientesFiltro { get; set; }

        [JsonProperty("codigo")]
        public string Codigo { get; set; }
    }
    public class ParamN
    {
        [JsonProperty("nPagina")]
        public long Pagina { get; set; }

        [JsonProperty("nRegPorPagina")]
        public long RegistrosPorPagina { get; set; } = 100;

        [JsonProperty("cExibeTodos")]
        public string ExibeTodos { get; set; }

        [JsonProperty("cNatureza")]
        public string Natureza { get; set; }

        [JsonProperty("dDtIncDe")]
        public string DtInicio { get; set; }

        [JsonProperty("dDtIncAte")]
        public string DtAte { get; set; }

        [JsonProperty("dDtAltDe")]
        public string DtAlteradoInicio { get; set; }

        [JsonProperty("dDtAltAte")]
        public string DtAlteradoAte { get; set; }


        [JsonProperty("nCodTabPreco")]
        public string CodTabPreco { get; set; }

        [JsonProperty("dDataPosicao")]
        public string DataPosicao { get; set; }
        [JsonProperty("codigo_local_estoque")]
        public string CodigoLocalEstoque { get; set; }

        [JsonProperty("lista_produtos")]
        public List<nCodProd> ListaProdutos { get; set; }


        public class nCodProd
        {
            [JsonProperty("nCodProd")]
            public string CodProd { get; set; }
        }

    }
  
    public class ClientesFiltros
    {
        public Tags Tag { get; set; }
        public ClientesFiltros(string tags = "Cliente")
        {
            Tags = new List<Tags>() { new Tags() { tag = tags } };
        }
        [JsonProperty("Tags")]
        public List<Tags> Tags { get; set; }
    }
    public class ParamTipoAtividade
    {
        [JsonProperty("filtrar_por_codigo")]
        public string FiltrarPorCodigo { get; set; } = "";

        [JsonProperty("filtrar_por_descricao")]
        public string FiltrarPorDescricao { get; set; } = "";
    }
}