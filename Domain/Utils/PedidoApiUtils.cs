using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Domain.Api.Parameters;
using Maxima.Cliente.Omie.Domain.Api.Requests;
using Maxima.Cliente.Omie.Domain.Api.Response;
using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Cliente.Omie.Domain.Enums;
using Maxima.Net.SDK.Integracao.Dto.Pedido;
using Maxima.Net.SDK.Integracao.Utils;

namespace Maxima.Cliente.Omie.Domain.Utils
{
    public static class PedidoApiUtils
    {
        public static async Task<ResponsePedidoOmie> GetStatusPedidoOmie(string idPedido, OmieContext dbContext)
        {
            ParamStatusPedido param = new ParamStatusPedido() { CodigoPedidoIntegracao = idPedido };

            var req = new RequestStatusPedidoOmie
            {

                AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                Call = "StatusPedido",
                ParamStatusPedido = new List<ParamStatusPedido>() { param }
            };

            return await ApiUtilsMaxima.RequisicaoAsync<ResponsePedidoOmie, RequestStatusPedidoOmie>(ConstantesEnum.UrlBaseOmie + PedidoOmie.VersaoAPI + PedidoOmie.Endpoint, HttpMethod.Post, req);
        }

        public static string MontarDescricaoCritica(ResponsePedidoOmie pedidoOmie)
        {
            return pedidoOmie.Cancelada.ToUpper().Equals("S") ? "Pedido Cancelado" : pedidoOmie.DescricaoStatus ?? "Pedido importado com sucesso!";
        }
        

        public static EnumPosicaoPedido RetornarPosicaoPedidoMaxima(ResponsePedidoOmie responsePedido, OmieContext dbContext)
        {
            var etapas = dbContext.ControleDadosModels.Where(e => e.Tabela == ControleDadosEnum.ETAPAPEDIDO && e.Chave == responsePedido.Etapa).FirstOrDefault();
            if (etapas == null)
                return EnumPosicaoPedido.Pendente;

            if (responsePedido.Cancelada.ToUpper().Equals("S"))
                return EnumPosicaoPedido.Cancelado;

            return etapas.Valor switch
            {
                "0" => EnumPosicaoPedido.Pendente,
                "2" => EnumPosicaoPedido.Liberado,
                "3" => EnumPosicaoPedido.Montado,
                "4" => EnumPosicaoPedido.Faturado,
                _ => EnumPosicaoPedido.Pendente
            };
        }

        public static string RetornarPosicaoHistoricoPedido(PedidoOmie pedidoOmie, OmieContext dbContext)
        {
            var etapas = dbContext.ControleDadosModels.Where(e => e.Tabela == ControleDadosEnum.ETAPAPEDIDO && e.Chave == pedidoOmie.cabecalho.etapa).FirstOrDefault();
            if (etapas == null)
                return "P";

            if (pedidoOmie.infoCadastro.cancelado.ToUpper().Equals("S"))
                return "C";

            return etapas.Valor switch
            {
                "0" => "P",
                "2" => "L",
                "3" => "M",
                "4" => "F",
                _ => "P"
            };
        }
    }
}