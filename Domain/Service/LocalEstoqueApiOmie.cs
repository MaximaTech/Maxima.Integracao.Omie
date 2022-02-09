using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Domain.Api.Response;
using Maxima.Cliente.Omie.Domain.Api.Parameters;
using Maxima.Cliente.Omie.Domain.Api.Requests;
using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Cliente.Omie.Domain.Enums;
using Maxima.Cliente.Omie.Domain.Interfaces.Api;
using Maxima.Net.SDK.Integracao.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Maxima.Cliente.Omie.Domain.Service
{
    public class LocalEstoqueApiOmie : ILocalEstoqueApiOmie
    {
        private readonly OmieContext dbContext;

        public LocalEstoqueApiOmie(OmieContext omieContext)
        {
            dbContext = omieContext;
        }

        public async Task<List<LocalEstoqueOmie>> RecuperarLocalEstoque()
        {
            try
            {
                long totalPaginas;
                var pagina = 1;
                var retorno = new List<LocalEstoqueOmie>();
                do
                {

                    var request = new RequestApiOmiePorN
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "ListarLocaisEstoque",
                        ParamsN = new List<ParamN> { new ParamN() { Pagina = pagina } }
                    };

                    var resultApi = await ApiUtilsMaxima.RequisicaoAsync<ResponseLocalEstoqueApiOmie, RequestApiOmiePorN>(LocalEstoqueOmie.UrlApi, HttpMethod.Post, request);

                    retorno.AddRange(resultApi.LocaisEstoque);
                    totalPaginas = resultApi.TotalDePaginas;

                    pagina++;
                } while (pagina <= totalPaginas);

                return retorno;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
