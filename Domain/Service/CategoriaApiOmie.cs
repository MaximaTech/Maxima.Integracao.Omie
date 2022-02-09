using AutoMapper;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Domain.Api.Response;
using Maxima.Cliente.Omie.Domain.Api.Parameters;
using Maxima.Cliente.Omie.Domain.Api.Requests;
using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Cliente.Omie.Domain.Enums;
using Maxima.Cliente.Omie.Domain.Interfaces.Api;
using Maxima.Net.SDK.Integracao.Api;
using Maxima.Net.SDK.Integracao.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Maxima.Cliente.Omie.Domain.Service
{
    public class CategoriaApiOmie : ICategoriaApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;
        public CategoriaApiOmie(OmieContext context, IMapper mappe, MaximaIntegracao maximaIntegracao)
        {
            dbContext = context;
            mapper = mappe;
            this.apiMaxima = maximaIntegracao;
        }

        public async Task<List<CategoriaOmie>> RecuperarCategoria()
        {

            try
            {
                long totalPaginas;
                var pagina = 1;
                var retorno = new List<CategoriaOmie>();
                do
                {

                    var request = new RequestOmie
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "ListarCategorias",
                       Params = new List<Param>() { new Param() { Pagina = pagina } }
                    };
                    
                    var resultApi = await ApiUtilsMaxima.RequisicaoAsync<ResponseCategoriaOmie, RequestOmie>(CategoriaOmie.UrlApi, HttpMethod.Post, request);
                    retorno.AddRange(resultApi.Categorias);
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
