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
    public class ProdutoApiOmie : IProdutoApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly IFornecedorApiOmie fornecedor;
        private readonly IProdutosPorFornecedorApiOmie produtosPorFornecedor;
        private readonly IFilialApiOmie filial;
        private readonly IPrecoProdutoApiOmie precoProdutoApiOmie;
        private readonly IEstoqueApiOmie estoqueApiOmie;
        private readonly MaximaIntegracao apiMaxima;
        public ProdutoApiOmie(OmieContext context, IMapper mappe,
            MaximaIntegracao maximaIntegracao,
            IFornecedorApiOmie fornecedor,
            IProdutosPorFornecedorApiOmie produtosPorFornecedor,
            IFilialApiOmie filial,
            IPrecoProdutoApiOmie precoProdutoApiOmie,
            IEstoqueApiOmie estoqueApiOmie)
        {
            dbContext = context;
            mapper = mappe;
            this.apiMaxima = maximaIntegracao;
            this.fornecedor = fornecedor;
            this.produtosPorFornecedor = produtosPorFornecedor;
            this.filial = filial;
            this.precoProdutoApiOmie = precoProdutoApiOmie;
            this.estoqueApiOmie = estoqueApiOmie;
        }
        public async Task EnviarProdutos(CancellationToken token)
        {
            LogApi log = new("Produtos");

            try
            {
                var produtosBd = await dbContext.ControleDadosModels
                    .Where(c => c.Tabela == ControleDadosEnum.PRODUTOS)
                    .AsNoTracking()
                    .ToListAsync();

                var produtoFornecedorBd = await dbContext.ControleDadosModels
                    .Where(c => c.Tabela == ControleDadosEnum.PRODUTOSFORNECEDOR)
                    .AsNoTracking()
                    .ToListAsync();

                var fornecedorBd = await dbContext.ControleDadosModels
                    .Where(c => c.Tabela == ControleDadosEnum.FORNECEDORES)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                var filialDb = await dbContext.ControleDadosModels
                    .Where(x => x.Tabela == ControleDadosEnum.FILIAIS)
                    .FirstOrDefaultAsync();

                if (fornecedorBd == null)
                {
                    await fornecedor.EnviarFornecedores(token);
                    fornecedorBd = await dbContext.ControleDadosModels
                        .Where(c => c.Tabela == ControleDadosEnum.FORNECEDORES)
                       .AsNoTracking()
                       .FirstOrDefaultAsync();
                }

                if (!produtoFornecedorBd.Any())
                {
                    await produtosPorFornecedor.EnviarProdutosPorFornecedor(token);
                    produtoFornecedorBd = await dbContext.ControleDadosModels
                        .Where(x => x.Tabela == ControleDadosEnum.FORNECEDORES)
                       .AsNoTracking()
                       .ToListAsync();
                }
                if (filialDb == null)
                {
                    await filial.EnviarFiliais(token);
                    filialDb = await dbContext.ControleDadosModels
                        .Where(x => x.Tabela == ControleDadosEnum.FILIAIS)
                        .FirstOrDefaultAsync();
                }

                var pagina = 1;
                long totalPaginas;
                long contOmie;
                var processados = new List<string>();
                do
                {
                    var listaIncluir = new List<ProdutoMaxima>();
                    var listaAlterar = new List<ProdutoMaxima>();
                    var request = new RequestOmie
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "ListarProdutos",
                        Params = new List<Param>() { new Param() { Pagina = pagina, FiltrarApenasOmie = "N", Inativo = "N" } }
                    };

                    var resultApi = await ApiUtilsMaxima.RequisicaoAsync<ResponseProdutoOmie, RequestOmie>(ProdutoOmie.UrlApi, HttpMethod.Post, request);

                    totalPaginas = resultApi.TotalDePaginas;
                    contOmie = resultApi.TotalDeRegistros;

                    foreach (var produtoOmie in resultApi.Produtos)
                    {
                        var produtoMap = mapper.Map<ProdutoMaxima>(produtoOmie);

                        var listCodFornec = produtoFornecedorBd
                           .Where(x => x.Valor.Equals(produtoMap.CodigoProduto))
                           .FirstOrDefault();

                        produtoMap.CodigoFornecedor = listCodFornec?.Chave ?? fornecedorBd?.Chave;
                        produtoMap.CodigoFilial = filialDb.Chave;


                        if (produtosBd.Any() && produtosBd.Any(x => x.Chave == produtoMap.CodigoProduto && x.Valor != produtoMap.Hash))
                        {
                            listaAlterar.Add(produtoMap);
                        }
                        else if (!produtosBd.Any(x => x.Chave == produtoMap.CodigoProduto))
                        {
                            listaIncluir.Add(produtoMap);
                        }
                    }

                    if (listaIncluir.Any())
                    {
                        ResponseApiMaxima<ProdutoMaxima> retornoApiMaxima = await apiMaxima.IncluirProdutos(listaIncluir);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoProduto.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                ControleDadosModel produtoModel = new()
                                {
                                    Tabela = ControleDadosEnum.PRODUTOS,
                                    Chave = item.CodigoProduto,
                                    Valor = item.Hash
                                };

                                dbContext.ControleDadosModels.Add(produtoModel);
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
                        ResponseApiMaxima<ProdutoMaxima> retornoApiMaxima = await apiMaxima.AlterarProdutos(listaAlterar);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoProduto.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {

                                var produtoModel = produtosBd.Where(x => x.Chave == item.CodigoProduto).FirstOrDefault();
                                if (produtoModel != null)
                                {
                                    produtoModel.Valor = item.Hash;
                                    dbContext.Update(produtoModel);
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

                var produtoList = await dbContext.ControleDadosModels
                    .Where(x => x.Tabela == ControleDadosEnum.PRODUTOS)
                    .AsNoTracking()
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(produtoList)
                    .ToList();

                var produtoRemove = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave) && f.Tabela == ControleDadosEnum.PRODUTOS)
                    .ToListAsync();

                if (excluidos.Any())
                {
                    var listaExclusao = produtoRemove
                        .Select(x => x.Valor);

                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarBancos(listaExclusao.ToArray());
                    if (retornoApiMaxima.Sucesso)
                    {
                        dbContext.ControleDadosModels.RemoveRange(produtoRemove);
                        await dbContext.SaveChangesAsync();

                        log.ExcluirOk(listaExclusao.Count());
                    }
                    else
                    {
                        dbContext.ChangeTracker.Clear();
                        log.ExcluirErro(listaExclusao.Count(), retornoApiMaxima.Error);
                    }
                }

                await estoqueApiOmie.EnviarEstoque(token);
                await precoProdutoApiOmie.EnviarPrecoProdutos(token);

            }
            catch (Exception ex)
            {
                log.GlobalError(ex.Message);
                throw;
            }
        }

    }
}
