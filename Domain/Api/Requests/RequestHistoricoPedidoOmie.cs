using System.Collections.Generic;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Requests
{
    public class RequestHistoricoPedidoOmie : RequestOmie
    {
        [JsonProperty("param")]
        public List<ParamHistoricoPedido> ParamStatusPedido { get; set; }
    }

    public class ParamHistoricoPedido
    {
        [JsonProperty("registros_por_pagina")]
        public long RegistrosPorPagina { get; set; } = 20;

        [JsonProperty("pagina")]
        public long Pagina { get; set; }

        [JsonProperty("apenas_importado_api")]
        public string ApenasImportadoApi { get; set; }

        [JsonProperty("filtrar_apenas_inclusao")]
        public string FiltrarApenasInclusao { get; set; }
        [JsonProperty("filtrar_por_data_de")]
        public string DataEntradaDe { get; set; }

        [JsonProperty("filtrar_por_data_ate")]
        public string DataEntradaAte { get; set; }

    }
}