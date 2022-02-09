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
using Maxima.Net.SDK.Integracao.Dto;
using Maxima.Net.SDK.Integracao.Entidades;
using Maxima.Net.SDK.Integracao.Utils;
using Microsoft.EntityFrameworkCore;

namespace Maxima.Cliente.Omie.Domain.Service
{
    public class EstoqueApiOmie : IEstoqueApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;

        public EstoqueApiOmie(OmieContext context, IMapper mappe, MaximaIntegracao maximaIntegracao)
        {
            dbContext = context;
            mapper = mappe;
            this.apiMaxima = maximaIntegracao;
        }

        public async Task EnviarEstoque(CancellationToken token)
        {
            LogApi log = new("Estoques");

            try
            {
                var estoqueDb = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.ESTOQUES).AsNoTracking().ToListAsync();
                var filialDB = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.FILIAIS).AsNoTracking().ToListAsync();
                var pagina = 1;
                long totalPaginas;
                long countOmie;
                var processados = new List<string>();

                do
                {
                    var listaIncluir = new List<EstoqueMaxima>();
                    var listaAlterar = new List<EstoqueMaxima>();
                    var localEstoquePadrao = dbContext.Parametros.Where(c => c.Nome == ConstantesEnum.LocalEstoqueOmie).FirstOrDefault();
                    var request = new RequestApiOmiePorN
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "ListarPosEstoque",
                        ParamsN = new List<ParamN>() { new ParamN() { Pagina = pagina, ExibeTodos = "S", DataPosicao = DateTime.Now.ToString("dd/MM/yyyy"), CodigoLocalEstoque = localEstoquePadrao.Valor } }
                    };

                    var resultApi = await ApiUtilsMaxima.RequisicaoAsync<ResponseEstoqueOmie, RequestApiOmiePorN>(ConsultaEstoqueOmie.UrlApi, HttpMethod.Post, request);

                    totalPaginas = resultApi.TotalDePaginas;
                    countOmie = resultApi.TotalDeRegistros;

                    foreach (var produto in resultApi.EstoqueProdutos)
                    {

                        var mapEstoque = mapper.Map<EstoqueMaxima>(produto);

                        if (filialDB.Any())
                            mapEstoque.CodigoFilial = filialDB.FirstOrDefault().Chave;

                        if (estoqueDb.Any() && estoqueDb.Any(x => x.Chave == mapEstoque.CodigoProduto && x.Valor != mapEstoque.Hash))
                        {
                            listaAlterar.Add(mapEstoque);
                        }
                        else if (!estoqueDb.Any(x => x.Chave == mapEstoque.CodigoProduto))
                        {
                            listaIncluir.Add(mapEstoque);
                        }
                    }

                    if (listaIncluir.Any())
                    {
                        ResponseApiMaxima<EstoqueMaxima> retornoApiMaxima = await apiMaxima.IncluirEstoques(listaIncluir);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoProduto.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                ControleDadosModel estoqueModel = new()
                                {
                                    Tabela = ControleDadosEnum.ESTOQUES,
                                    Chave = item.CodigoProduto,
                                    Valor = item.Hash
                                };
                                dbContext.ControleDadosModels.Add(estoqueModel);
                            }
                            await dbContext.SaveChangesAsync();
                            log.InserirOk(pagina, totalPaginas, retornoApiMaxima.ItensInserido.Count());

                            if (retornoApiMaxima.ErrosValidacao.Any())
                                log.InserirErro(pagina, totalPaginas, retornoApiMaxima.TotalItensNaoInserido, retornoApiMaxima.ErrosValidacaoFormatado);
                        }
                        else
                        {
                            log.InserirErro(pagina, totalPaginas, listaIncluir.Count(), retornoApiMaxima.ErrosValidacaoFormatado);
                        }
                    }

                    if (listaAlterar.Any())
                    {
                        ResponseApiMaxima<EstoqueMaxima> retornoApiMaxima = await apiMaxima.AlterarEstoques(listaAlterar);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoProduto.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                var estoqueModel = estoqueDb.Where(x => x.Chave == item.CodigoProduto).FirstOrDefault();
                                if (estoqueModel != null)
                                {
                                    estoqueModel.Valor = item.Hash;
                                    dbContext.Update(estoqueModel);
                                }
                            }
                            await dbContext.SaveChangesAsync();
                            log.InserirOk(pagina, totalPaginas, retornoApiMaxima.ItensInserido.Count());

                            if (retornoApiMaxima.ErrosValidacao.Any())
                                log.InserirErro(pagina, totalPaginas, retornoApiMaxima.TotalItensNaoInserido, retornoApiMaxima.ErrosValidacaoFormatado);
                        }
                        else
                        {
                            log.AlterarErro(pagina, totalPaginas, listaAlterar.Count, retornoApiMaxima.ErrosValidacaoFormatado);
                        }
                    }

                    if (!listaAlterar.Any() && !listaIncluir.Any())
                        log.NenhumRegistroAlterado(pagina, totalPaginas);

                    pagina++;
                } while (pagina <= totalPaginas);

                var estoqueList = await dbContext.ControleDadosModels
                    .Where(x => x.Tabela == ControleDadosEnum.ESTOQUES)
                    .AsNoTracking()
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(estoqueList)
                    .ToList();

                var estoqueRemove = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave) && f.Tabela == ControleDadosEnum.ESTOQUES)
                    .ToListAsync();

                if (excluidos.Any())
                {
                    dbContext.ControleDadosModels.RemoveRange(estoqueRemove);

                    var listaExclusao = estoqueRemove.Select(x => x.Valor);
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarEstoque(listaExclusao.ToArray());
                    if (retornoApiMaxima.Sucesso)
                    {
                        dbContext.ControleDadosModels.RemoveRange(estoqueRemove);
                        await dbContext.SaveChangesAsync();
                        log.ExcluirOk(listaExclusao.Count());
                    }
                    else
                    {
                        dbContext.ChangeTracker.Clear();
                        log.ExcluirErro(listaExclusao.Count(), retornoApiMaxima.Error);
                    }

                }
            }
            catch (Exception ex)
            {
                log.GlobalError(ex.Message);
                throw;
            }
        }
    }
}
