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
    public class FornecedorApiOmie : IFornecedorApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;

        public FornecedorApiOmie(OmieContext context, IMapper mappe, MaximaIntegracao maximaIntegracao)
        {
            dbContext = context;
            mapper = mappe;
            this.apiMaxima = maximaIntegracao;
        }

        public async Task EnviarFornecedores(CancellationToken token)
        {
            LogApi log = new("Fornecedores");

            try
            {
                var fornecedorBd = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.FORNECEDORES).AsNoTracking().ToListAsync();
                var processados = new List<string>();
                var pagina = 1;
                long totalPaginas;
                long countOmie;

                do
                {
                    var listaIncluir = new List<FornecedorMaxima>();
                    var listaAlterar = new List<FornecedorMaxima>();

                    var request = new RequestOmie
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "ListarClientes",
                        Params = new List<Param> { new Param() { Pagina = pagina, ClientesFiltro = new ClientesFiltros(tags: "Fornecedor") } }
                    };

                    var result = await ApiUtilsMaxima.RequisicaoAsync<ResponseClienteOmie, RequestOmie>(ClienteOmie.UrlApi, HttpMethod.Post, request);

                    totalPaginas = result.TotalDePaginas;
                    countOmie = result.TotalDeRegistros;

                    foreach (var fornecedor in result.Clientes)
                    {
                        var mapFornecedor = mapper.Map<FornecedorMaxima>(fornecedor);

                        if (fornecedorBd.Any() && fornecedorBd.Any(x => x.Chave == mapFornecedor.CodigoFornecedor && x.Valor != mapFornecedor.Hash))
                        {
                            listaAlterar.Add(mapFornecedor);
                        }
                        else if (!fornecedorBd.Any(x => x.Chave == mapFornecedor.CodigoFornecedor))
                        {
                            listaIncluir.Add(mapFornecedor);
                        }
                    }

                    if (listaIncluir.Any())
                    {
                        ResponseApiMaxima<FornecedorMaxima> retornoApiMaxima = await apiMaxima.IncluirFornecedor(listaIncluir);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoFornecedor.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                ControleDadosModel fornecedorModel = new()
                                {
                                    Tabela = ControleDadosEnum.FORNECEDORES,
                                    Chave = item.CodigoFornecedor,
                                    Valor = item.Hash
                                };
                                dbContext.ControleDadosModels.Add(fornecedorModel);
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
                        ResponseApiMaxima<FornecedorMaxima> retornoApiMaxima = await apiMaxima.AlterarFornecedor(listaAlterar);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoFornecedor.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                var fornecedorModel = fornecedorBd.Where(c => c.Chave == item.CodigoFornecedor).FirstOrDefault();
                                if (fornecedorModel != null)
                                {
                                    fornecedorModel.Valor = item.Hash;
                                    dbContext.Update(fornecedorModel);
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

                var fornecedorList = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.FORNECEDORES)
                    .AsNoTracking()
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(fornecedorList)
                    .ToList();

                var fornecedorRemove = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.FORNECEDORES)
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave))
                    .ToListAsync();

                if (excluidos.Any())
                {
                    var listaExclusao = fornecedorRemove.Select(x => x.Valor);
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarFornecedor(listaExclusao.ToArray());
                    if (retornoApiMaxima.Sucesso)
                    {
                        dbContext.ControleDadosModels.RemoveRange(fornecedorRemove);
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

