using System.Collections.Generic;
using Maxima.Cliente.Omie.Domain.Entidades;
using Newtonsoft.Json;

namespace Maxima.Cliente.Omie.Domain.Api.Response
{
    public class ResponseClienteCadastroOmie
    {

        [JsonProperty("codigo_cliente_omie")]
        public long codigo_cliente_omie { get; set; }

        [JsonProperty("codigo_cliente_integracao")]
        public string codigo_cliente_integracao { get; set; }

        [JsonProperty("codigo_status")]
        public string codigo_status { get; set; }

        [JsonProperty("descricao_status")]
        public string descricao_status { get; set; }

        [JsonProperty("faultstring")]
        public string faultstring { get; set; }

        [JsonProperty("faultcode")]
        public string faultcode { get; set; }

        public string Erro
        {
            get
            {
                return faultcode + " " + faultstring;
            }
        }
        public bool Sucesso
        {
            get
            {
                return faultcode == null;
            }
        }
    }
}