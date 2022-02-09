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
    public class EtapaPedidoApiOmie : IEtapaPedidoApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;

        public EtapaPedidoApiOmie(OmieContext context, IMapper mappe, MaximaIntegracao maximaIntegracao)
        {
            dbContext = context;
            mapper = mappe;
            this.apiMaxima = maximaIntegracao;
        }

        public async Task<List<EtapaPedidoOmie>> RecuperarEtapaOmie()
        {
            try
            {
                long totalPaginas;
                var pagina = 1;
                var retorno = new List<EtapaPedidoOmie>();
                do
                {
                    var request = new RequestOmie
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "ListarEtapasFaturamento",
                        Params = new List<Param> { new Param() { Pagina = pagina, ApenasImportadoApi = null } }
                    };

                    var resultApi = await ApiUtilsMaxima.RequisicaoAsync<ResponseEtapaPedidoOmie, RequestOmie>(EtapasFaturamentoOmie.UrlApi, HttpMethod.Post, request);

                    List<EtapasFaturamentoOmie> listCategorias = resultApi.EtapasOmie.Where(c => c.cCodOperacao == "11").ToList();
                    foreach (var etapaFaturamento in listCategorias)
                    {
                        foreach (var etapas in etapaFaturamento.etapas)
                        {
                            EtapaPedidoOmie etapaPedido = new();
                            etapaPedido.Key = etapas.cCodigo;
                            etapaPedido.Value = etapas.cDescricao;
                            etapaPedido.EtapasMaxima = ApiUtilsMaxima.RecuperarEtapaMaxima();
                            retorno.Add(etapaPedido);
                        }
                        dbContext.SaveChanges();
                    }
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
