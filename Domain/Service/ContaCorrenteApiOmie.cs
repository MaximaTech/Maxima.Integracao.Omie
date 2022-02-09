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
    public class ContaCorrenteApiOmie : IContaCorrenteApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;
        public ContaCorrenteApiOmie(OmieContext context, IMapper mappe, MaximaIntegracao maximaIntegracao)
        {
            dbContext = context;
            mapper = mappe;
            this.apiMaxima = maximaIntegracao;
        }

        public async Task<Dictionary<string, string>> RecuperarContaCorrente()
        {
            try
            {
                var pagina = 1;
                var retorno = new Dictionary<string, string>();
                var request = new RequestOmie
                {
                    AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                    AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                    Call = "PesquisarContaCorrente",
                    Params = new List<Param>() { new Param() { Pagina = pagina } }
                };

                var resultApi = await ApiUtilsMaxima.RequisicaoAsync<ResponseContaCorrenteOmie, RequestOmie>(ContaCorrenteOmie.UrlApi, HttpMethod.Post, request);

                foreach (var contaCorrente in resultApi.ContasCorrentes)
                {
                    retorno.Add(contaCorrente.nCodCC.ToString(), contaCorrente.descricao);
                }

                return retorno;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
