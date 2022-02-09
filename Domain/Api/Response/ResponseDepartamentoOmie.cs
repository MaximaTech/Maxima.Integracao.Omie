using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseDepartamentoOmie : ResponseOmie
    {
        public ResponseDepartamentoOmie()
        {
            Departamentos = new List<DepartamentoOmie>();
        }
        [JsonProperty("departamentos")]
        public List<DepartamentoOmie> Departamentos { get; set; }
    }
}
