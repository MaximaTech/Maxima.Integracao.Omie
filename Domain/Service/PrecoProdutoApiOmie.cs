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
    public class PrecoProdutoApiOmie : IPrecoProdutoApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;
        private readonly IContaCorrenteApiOmie _contaCorrenteApiOmie;

        public PrecoProdutoApiOmie(OmieContext context, IMapper mappe, MaximaIntegracao maximaIntegracao, IContaCorrenteApiOmie contaCorrenteApiOmie)
        {
            dbContext = context;
            mapper = mappe;
            this.apiMaxima = maximaIntegracao;
            _contaCorrenteApiOmie = contaCorrenteApiOmie;
        }

        public async Task EnviarPrecoProdutos(CancellationToken token)
        {
            LogApi log = new("Preços Produtos");

            try
            {
                var tabelaPrecoBd = await dbContext.ControleDadosModels
                    .Where(x => x.Tabela == ControleDadosEnum.PRECOPRODUTO)
                    .AsNoTracking()
                    .ToListAsync();

                var processados = new List<string>();

                var pagina = 1;

                long totalPaginas;
                long countOmie;

                do
                {
                    var listaIncluir = new List<TabelaPrecoMaxima>();
                    var listaAlterar = new List<TabelaPrecoMaxima>();

                    var request = new RequestApiOmiePorN
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "ListarTabelaItens",
                        ParamsN = new List<ParamN>()
                    };
                    request.ParamsN.Add(new ParamN()
                    {
                        Pagina = pagina,
                    });
                    var resultApi = await ApiUtilsMaxima.RequisicaoAsync<ResponseTabeladePrecoOmie, RequestApiOmiePorN>(TabelaPrecosOmie.UrlApi, HttpMethod.Post, request);
                    totalPaginas = resultApi.TotalDePaginas;

                    countOmie = resultApi.TotalDeRegistros;

                    foreach (var tabelaPreco in resultApi.TabelasPrecoItens.itensTabela)
                    {
                        TabelaPrecoMaxima mapTabelaPreco = mapper.Map<TabelaPrecoMaxima>(tabelaPreco);
                        mapTabelaPreco.CodigoRegiao = resultApi.TabelasPrecoItens.nCodTabPreco;

                        if (tabelaPrecoBd.Any() && tabelaPrecoBd.Any(x => x.Chave == mapTabelaPreco.CodigoProduto && x.Valor != mapTabelaPreco.Hash))
                        {
                            listaAlterar.Add(mapTabelaPreco);
                        }
                        else if (!tabelaPrecoBd.Any(x => x.Chave == mapTabelaPreco.CodigoProduto))
                        {
                            listaIncluir.Add(mapTabelaPreco);
                        }
                    }

                    if (listaIncluir.Any())
                    {
                        ResponseApiMaxima<TabelaPrecoMaxima> retornoApiMaxima = await apiMaxima.IncluirTabelaPreco(listaIncluir);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoProduto.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                ControleDadosModel tabelaPrecoModel = new()
                                {
                                    Tabela = ControleDadosEnum.PRECOPRODUTO,
                                    Chave = item.CodigoProduto,
                                    Valor = item.Hash
                                };
                                dbContext.ControleDadosModels.Add(tabelaPrecoModel);
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
                        ResponseApiMaxima<TabelaPrecoMaxima> retornoApiMaxima = await apiMaxima.AlterarTabelaPreco(listaAlterar);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoProduto.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                var tabelaPrecoModel = tabelaPrecoBd.Where(c => c.Chave == item.CodigoProduto).FirstOrDefault();
                                if (tabelaPrecoModel != null)
                                {
                                    tabelaPrecoModel.Valor = item.Hash;
                                    dbContext.Update(tabelaPrecoModel);
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

                var tabelaPrecoList = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(w => w.Tabela == ControleDadosEnum.PRECOPRODUTO)
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(tabelaPrecoList)
                    .ToList();

                var tabelaPrecoRemove = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave) && f.Tabela == ControleDadosEnum.PRECOPRODUTO)
                    .ToListAsync();

                if (tabelaPrecoRemove.Any())
                {
                    var listaExclusao = tabelaPrecoRemove.Select(x => x.Valor);
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarTabelaPreco(listaExclusao.ToArray());
                    if (retornoApiMaxima.Sucesso)
                    {
                        dbContext.ControleDadosModels.RemoveRange(tabelaPrecoRemove);
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
