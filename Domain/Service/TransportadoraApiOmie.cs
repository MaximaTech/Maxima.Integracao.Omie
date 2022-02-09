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
using static Maxima.Cliente.Omie.Domain.Entidades.ContaOmie;

namespace Maxima.Cliente.Omie.Domain.Service
{
    public class TransportadoraApiOmie : ITransportadoraApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;

        public TransportadoraApiOmie(OmieContext context, IMapper mapper, MaximaIntegracao maximaIntegracao)
        {
            dbContext = context;
            this.mapper = mapper;
            apiMaxima = maximaIntegracao;
        }

        public async Task EnviarTransportadoras(CancellationToken token)
        {
            LogApi log = new("Transportadoras");

            try
            {
                var transportadoraBd = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.TRANSPORTADORAS).AsNoTracking().ToListAsync();

                var processados = new List<string>();
                var pagina = 1;
                long totalPaginas;
                long countOmie;
                do
                {
                    var listaIncluir = new List<TransportadoraMaxima>();
                    var listaAlterar = new List<TransportadoraMaxima>();
                    var request = new RequestOmie
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "ListarClientes",
                        Params = new List<Param>() { new Param() { Pagina = pagina, ClientesFiltro = new ClientesFiltros() { Tags = new List<Tags>() { new Tags() { tag = "Transportadora" } } } } }
                    };

                    var result = await ApiUtilsMaxima.RequisicaoAsync<ResponseClienteOmie, RequestOmie>(ClienteOmie.UrlApi, HttpMethod.Post, request);

                    totalPaginas = result.TotalDePaginas;
                    countOmie = result.TotalDeRegistros;

                    foreach (var transportadoras in result.Clientes)
                    {
                        var mapTransportadora = mapper.Map<TransportadoraMaxima>(transportadoras);

                        if (transportadoraBd.Any(x => x.Chave == mapTransportadora.CodigoTrasportadora && x.Valor != mapTransportadora.Hash))
                        {
                            listaAlterar.Add(mapTransportadora);
                        }
                        else if (!transportadoraBd.Any(x => x.Chave == mapTransportadora.CodigoTrasportadora))
                        {
                            listaIncluir.Add(mapTransportadora);
                        }
                    }

                    if (listaIncluir.Any())
                    {
                        ResponseApiMaxima<TransportadoraMaxima> retornoApiMaxima = await apiMaxima.IncluirTransportadora(listaIncluir);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoTrasportadora.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                ControleDadosModel transportadoraModel = new()
                                {
                                    Tabela = ControleDadosEnum.TRANSPORTADORAS,
                                    Chave = item.CodigoTrasportadora,
                                    Valor = item.Hash
                                };
                                dbContext.ControleDadosModels.Add(transportadoraModel);
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
                        ResponseApiMaxima<TransportadoraMaxima> retornoApiMaxima = await apiMaxima.AlterarTransportadora(listaAlterar);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoTrasportadora.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                var transportadoraModel = transportadoraBd.Where(c => c.Chave == item.CodigoTrasportadora).FirstOrDefault();
                                if (transportadoraModel != null)
                                {
                                    transportadoraModel.Valor = item.Hash;
                                    dbContext.Update(transportadoraModel);
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

                var trasnportadoraList = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.TRANSPORTADORAS)
                    .AsNoTracking()
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(trasnportadoraList)
                    .ToList();

                var transportadoraRemove = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.TRANSPORTADORAS)
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave))
                    .ToListAsync();

                if (excluidos.Any())
                {
                    var listaExclusao = transportadoraRemove.Select(x => x.Valor);
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarTransportadoras(listaExclusao.ToArray());
                    if (retornoApiMaxima.Sucesso)
                    {
                        dbContext.ControleDadosModels.RemoveRange(transportadoraRemove);
                        await dbContext.SaveChangesAsync();
                        log.ExcluirOk(listaExclusao.Count());
                    }
                    else
                    {
                        dbContext.ChangeTracker.Clear();
                        log.ExcluirErro(listaExclusao.Count(), retornoApiMaxima.Error);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                log.GlobalError(ex.Message);
                throw;
            }

        }

    }
}
