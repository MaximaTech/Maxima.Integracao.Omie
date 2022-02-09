using AutoMapper;
using System.Threading;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Data.Models;
using Maxima.Cliente.Omie.Domain.Api.Response;
using Maxima.Cliente.Omie.Domain.Api.Parameters;
using Maxima.Cliente.Omie.Domain.Api.Requests;
using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Cliente.Omie.Domain.Enums;
using Maxima.Cliente.Omie.Domain.Interfaces.Api;
using Maxima.Cliente.Omie.Domain.Utils;
using Maxima.Net.SDK.Integracao.Api;
using Maxima.Net.SDK.Integracao.Dto;
using Maxima.Net.SDK.Integracao.Entidades;
using Maxima.Net.SDK.Integracao.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Maxima.Cliente.Omie.Domain.Service
{
    public class DepartamentoApiOmie : IDepartamentoApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;
        public DepartamentoApiOmie(OmieContext context, IMapper mapper, MaximaIntegracao maximaIntegracao)
        {
            this.dbContext = context;
            this.mapper = mapper;
            apiMaxima = maximaIntegracao;
        }

        public async Task ListarDepartamentos(CancellationToken token)
        {
            LogApi log = new("Departamentos");

            try
            {
                var departamentoBd = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.DEPARTAMENTOS).AsNoTracking().ToListAsync();
                var processados = new List<string>();
                var pagina = 1;
                long totalPaginas;
                long countOmie;

                do
                {
                    var listaIncluir = new List<DepartamentoMaxima>();
                    var listaAlterar = new List<DepartamentoMaxima>();

                    var request = new RequestOmie
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "ListarDepartamentos",
                        Params = new List<Param>() { new Param() { Pagina = pagina } }
                    };

                    var result = await ApiUtilsMaxima.RequisicaoAsync<ResponseDepartamentoOmie, RequestOmie>(DepartamentoOmie.UrlApi, HttpMethod.Post, request);

                    totalPaginas = result.TotalDePaginas;
                    countOmie = result.TotalDeRegistros;

                    foreach (var departamento in result.Departamentos)
                    {
                        var mapDepartamento = mapper.Map<DepartamentoMaxima>(departamento);

                        if (departamentoBd.Any() && departamentoBd.Any(x => x.Chave == mapDepartamento.CodigoDepartamento && x.Valor != mapDepartamento.Hash))
                        {
                            listaAlterar.Add(mapDepartamento);
                        }
                        else if (!departamentoBd.Any(x => x.Chave == mapDepartamento.CodigoDepartamento))
                        {
                            listaIncluir.Add(mapDepartamento);
                        }
                    }

                    if (listaIncluir.Any())
                    {
                        ResponseApiMaxima<DepartamentoMaxima> retornoApiMaxima = await apiMaxima.IncluirDepartamentos(listaIncluir);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoDepartamento).ToList());

                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                ControleDadosModel departamentoModel = new()
                                {
                                    Tabela = ControleDadosEnum.DEPARTAMENTOS,
                                    Chave = item.CodigoDepartamento,
                                    Valor = item.Hash
                                };
                                dbContext.ControleDadosModels.Add(departamentoModel);
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
                        ResponseApiMaxima<DepartamentoMaxima> retornoApiMaxima = await apiMaxima.AlterarDepartamentos(listaAlterar);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoDepartamento).ToList());

                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                var departamentoModel = departamentoBd.Where(x => x.Chave == item.CodigoDepartamento).FirstOrDefault();
                                if (departamentoModel != null)
                                {
                                    departamentoModel.Valor = item.Hash;
                                    dbContext.Update(departamentoModel);
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

                var departamentoList = await dbContext.ControleDadosModels.
                    Where(c => c.Tabela == ControleDadosEnum.DEPARTAMENTOS)
                    .AsNoTracking()
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(departamentoList)
                    .ToList();

                var departamentoRemove = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave) && f.Tabela == ControleDadosEnum.DEPARTAMENTOS)
                    .ToListAsync();

                if (excluidos.Any())
                {
                    var listaExclusao = departamentoRemove.Select(x => x.Valor);

                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarDepartamentos(listaExclusao.ToArray());
                    if (retornoApiMaxima.Sucesso)
                    {
                        dbContext.ControleDadosModels.RemoveRange(departamentoRemove);
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
