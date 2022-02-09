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
    public class FormaPagamentoApiOmie : IFormaPagamentoApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;
        public FormaPagamentoApiOmie(OmieContext context, IMapper mappe, MaximaIntegracao maximaIntegracao)
        {
            dbContext = context;
            mapper = mappe;
            this.apiMaxima = maximaIntegracao;
        }

        public async Task EnviarFormaDePagamento(CancellationToken token)
        {
            LogApi log = new("Formas de Pagamentos");

            try
            {
                var formasDePagamentoBd = await dbContext.ControleDadosModels
                    .Where(x => x.Tabela == ControleDadosEnum.FORMAPAGAMENTO)
                    .AsNoTracking()
                    .ToListAsync();
                var processados = new List<string>();
                var pagina = 1;
                long totalPaginas;
                long CountOmie;

                do
                {
                    var listaIncluir = new List<PlanoPagamentoMaxima>();
                    var listaAlterar = new List<PlanoPagamentoMaxima>();

                    var request = new RequestOmie
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "ListarFormasPagVendas",
                        Params = new List<Param> { new Param() { Pagina = pagina, ApenasImportadoApi = null } }
                    };
                    var resultApi = await ApiUtilsMaxima.RequisicaoAsync<ResponseFormaPagamento, RequestOmie>(FormaPagamentoOmie.UrlApi, HttpMethod.Post, request);

                    totalPaginas = resultApi.TotalDePaginas;
                    CountOmie = resultApi.TotalDeRegistros;

                    foreach (var formaPagamento in resultApi.FormaPagamentos)
                    {

                        var mapFormaPagamento = mapper.Map<PlanoPagamentoMaxima>(formaPagamento);

                        if (formasDePagamentoBd.Any() && formasDePagamentoBd.Any(x => x.Chave == mapFormaPagamento.CodigoPlanoPagamento && x.Valor != mapFormaPagamento.Hash))
                        {
                            listaAlterar.Add(mapFormaPagamento);
                        }
                        else if (!formasDePagamentoBd.Any(x => x.Chave == mapFormaPagamento.CodigoPlanoPagamento))
                        {
                            listaIncluir.Add(mapFormaPagamento);
                        }
                    }

                    if (listaIncluir.Any())
                    {
                        ResponseApiMaxima<PlanoPagamentoMaxima> retornoApiMaxima = await apiMaxima.IncluirPlanoPagamento(listaIncluir);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoPlanoPagamento).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                ControleDadosModel formaDePagamentoModel = new()
                                {
                                    Tabela = ControleDadosEnum.FORMAPAGAMENTO,
                                    Chave = item.CodigoPlanoPagamento,
                                    Valor = item.Hash
                                };
                                dbContext.ControleDadosModels.Add(formaDePagamentoModel);
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
                        ResponseApiMaxima<PlanoPagamentoMaxima> retornoApiMaxima = await apiMaxima.AlterarPlanoPagamento(listaAlterar);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoPlanoPagamento).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                var formaDePagamentoModel = formasDePagamentoBd.Where(c => c.Chave == item.CodigoPlanoPagamento).FirstOrDefault();
                                if (formaDePagamentoModel != null)
                                {
                                    formaDePagamentoModel.Valor = item.Hash;
                                    dbContext.Update(formaDePagamentoModel);
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

                var formaPagamentoList = await dbContext.ControleDadosModels
                    .Where(x => x.Tabela == ControleDadosEnum.FORMAPAGAMENTO)
                    .AsNoTracking()
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(formaPagamentoList)
                    .ToList();

                var formaPagamentoRemove = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave) && f.Tabela == ControleDadosEnum.FORMAPAGAMENTO)
                    .ToListAsync();

                if (excluidos.Any())
                {
                    var listaExclusao = formaPagamentoRemove.Select(x => x.Valor);
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarPlanoPagamento(listaExclusao.ToArray());
                    if (retornoApiMaxima.Sucesso)
                    {
                        dbContext.ControleDadosModels.RemoveRange(formaPagamentoRemove);
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
