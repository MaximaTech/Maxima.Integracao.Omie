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
    public class CidadeApiOmie : ICidadeApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;


        public CidadeApiOmie(OmieContext context, IMapper mapper, MaximaIntegracao maximaIntegracao)
        {
            dbContext = context;
            this.mapper = mapper;
            apiMaxima = maximaIntegracao;
        }

        public async Task EnviarCidades(CancellationToken token)
        {
            LogApi log = new("Cidades");

            try
            {
                var cidadesBd = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.CIDADES).AsNoTracking().ToListAsync();
                var processados = new List<string>();
                var pagina = 1;
                long totalPaginas;
                long countOmie;

                do
                {
                    var listaIncluir = new List<CidadeMaxima>();
                    var listaAlterar = new List<CidadeMaxima>();

                    var request = new RequestOmie
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "PesquisarCidades",
                        Params = new List<Param>() { new Param() { Pagina = pagina } }
                    };

                    var result = await ApiUtilsMaxima.RequisicaoAsync<ResponseCidadesOmie, RequestOmie>(CidadeOmie.UrlApi, HttpMethod.Post, request);

                    totalPaginas = result.TotalDePaginas;
                    countOmie = result.TotalDeRegistros;

                    foreach (var cidades in result.Cidades)
                    {
                        var mapCidades = mapper.Map<CidadeMaxima>(cidades);

                        if (cidadesBd.Any(x => x.Chave == mapCidades.CodigoCidade && x.Valor != mapCidades.Hash))
                        {
                            listaAlterar.Add(mapCidades);
                        }
                        else if (!cidadesBd.Any(x => x.Chave == mapCidades.CodigoCidade))
                        {
                            listaIncluir.Add(mapCidades);
                        }
                    }

                    if (listaIncluir.Any())
                    {
                        ResponseApiMaxima<CidadeMaxima> retornoApiMaxima = await apiMaxima.IncluirCidade(listaIncluir);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoCidade).ToList());

                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                ControleDadosModel cidadeModel = new()
                                {
                                    Tabela = ControleDadosEnum.CIDADES,
                                    Chave = item.CodigoCidade,
                                    Valor = item.Hash
                                };
                                dbContext.ControleDadosModels.Add(cidadeModel);
                            }
                            await dbContext.SaveChangesAsync();

                            log.InserirOk(pagina, totalPaginas, retornoApiMaxima.ItensInserido.Count());

                            if (retornoApiMaxima.ErrosValidacao.Any())
                                log.InserirErro(pagina, totalPaginas, retornoApiMaxima.TotalItensNaoInserido, retornoApiMaxima.ErrosValidacaoFormatado);
                        }
                        else
                        {
                            log.InserirErro(pagina, totalPaginas, listaIncluir.Count, retornoApiMaxima.ErrosValidacaoFormatado);
                        }
                    }

                    if (listaAlterar.Any())
                    {
                        ResponseApiMaxima<CidadeMaxima> retornoApiMaxima = await apiMaxima.AlterarCidade(listaIncluir);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoCidade).ToList());

                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                var cidadeBd = cidadesBd.Where(c => c.Chave == item.CodigoCidade).FirstOrDefault();
                                dbContext.Update(cidadeBd);
                            }
                            await dbContext.SaveChangesAsync();

                            log.InserirOk(pagina, totalPaginas, retornoApiMaxima.ItensInserido.Count());

                            if (retornoApiMaxima.ErrosValidacao.Any())
                                log.InserirErro(pagina, totalPaginas, retornoApiMaxima.TotalItensNaoInserido, retornoApiMaxima.ErrosValidacaoFormatado);
                        }
                        else
                        {
                            log.InserirErro(pagina, totalPaginas, listaIncluir.Count, retornoApiMaxima.ErrosValidacaoFormatado);
                        }
                    }

                    if (!listaAlterar.Any() && !listaIncluir.Any())
                        log.NenhumRegistroAlterado(pagina, totalPaginas);

                    pagina++;
                } while (pagina <= totalPaginas);

                var cidadeList = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.CIDADES)
                    .AsNoTracking()
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(cidadeList)
                    .ToList();

                var cidadeRemove = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.CIDADES)
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave))
                    .ToListAsync();

                if (excluidos.Any())
                {
                    var listaExclusao = cidadeRemove.Select(x => x.Valor);
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarCidades(listaExclusao.ToArray());
                    if (retornoApiMaxima.Sucesso)
                    {
                        dbContext.ControleDadosModels.RemoveRange(cidadeRemove);
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
