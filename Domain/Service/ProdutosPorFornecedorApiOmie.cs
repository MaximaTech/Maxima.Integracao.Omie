using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Data.Models;
using Maxima.Cliente.Omie.Domain.Api.Parameters;
using Maxima.Cliente.Omie.Domain.Api.Requests;
using Maxima.Cliente.Omie.Domain.Api.Response;
using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Cliente.Omie.Domain.Enums;
using Maxima.Cliente.Omie.Domain.Interfaces.Api;
using Maxima.Cliente.Omie.Domain.Utils;
using Maxima.Net.SDK.Integracao.Api;
using Maxima.Net.SDK.Integracao.Utils;
using Microsoft.EntityFrameworkCore;

namespace Maxima.Cliente.Omie.Domain.Service
{
    public class ProdutosPorFornecedorApiOmie : IProdutosPorFornecedorApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;

        private readonly MaximaIntegracao apiMaxima;
        public ProdutosPorFornecedorApiOmie(OmieContext context, IMapper mappe, MaximaIntegracao maximaIntegracao)
        {
            dbContext = context;
            mapper = mappe;
            this.apiMaxima = maximaIntegracao;
        }

        public async Task EnviarProdutosPorFornecedor(CancellationToken token)
        {
            LogApi log = new("Produtos x Fornecedores");

            try
            {

                var estoqueDb = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.PRODUTOSFORNECEDOR).AsNoTracking().ToListAsync();
                var pagina = 1;
                long totalPaginas;
                var processados = new List<string>();
                do
                {
                    var request = new RequestOmie
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "ListarProdutoFornecedor",
                        Params = new List<Param>() { new Param() { Pagina = pagina, OrdemDecrescente = "S" } }
                    };

                    var resultApi = await ApiUtilsMaxima.RequisicaoAsync<ResponseProdutoFornecedorOmie, RequestOmie>(ProdutosPorFornecedorOmie.UrlApi, HttpMethod.Post, request);

                    totalPaginas = resultApi.TotalDePaginas;

                    foreach (var res in resultApi.ProdutoxFornecedorList)
                    {

                        foreach (var item in res.produtos)
                        {
                            ControleDadosModel produtosPorFornecedorModel = new()
                            {
                                Tabela = ControleDadosEnum.PRODUTOSFORNECEDOR,
                                Chave = res.nCodForn.ToString(),
                                Valor = item.nCodProd.ToString()
                            };

                            if (item.nCodProd != 0 && res.nCodForn != 0)
                            {
                                ControleDadosModel produtosPorFornecedor = await dbContext.ControleDadosModels
                                .Where(p => p.Chave == res.nCodForn.ToString() && p.Valor == item.nCodProd.ToString() && p.Tabela == ControleDadosEnum.PRODUTOSFORNECEDOR).FirstOrDefaultAsync();

                                if (produtosPorFornecedor == null)
                                {
                                    dbContext.ControleDadosModels.Add(produtosPorFornecedorModel);
                                }
                                else
                                {
                                    produtosPorFornecedor.Valor = produtosPorFornecedorModel.Valor;

                                }
                            }
                        }
                    }
                    int totalInserido = await dbContext.SaveChangesAsync();
                    log.InserirOk(pagina, totalPaginas, totalInserido);

                    pagina++;

                } while (pagina <= totalPaginas);
            }
            catch (Exception ex)
            {
                log.GlobalError(ex.Message);
                throw;
            }
        }
    }
}
