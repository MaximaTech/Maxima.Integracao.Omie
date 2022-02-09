using System.Collections.Generic;
using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseOmie
    {

        [JsonProperty("pagina")]
        public long Pagina { get; set; }

        [JsonProperty("total_de_paginas")]
        public long TotalDePaginas { get; set; }

        [JsonProperty("registros")]
        public long Registros { get; set; }

        [JsonProperty("total_de_registros")]
        public long TotalDeRegistros { get; set; }

        [JsonProperty("faultstring")]
        public string faultstring { get; set; }

        [JsonProperty("faultcode")]
        public string faultcode { get; set; }

    }
}