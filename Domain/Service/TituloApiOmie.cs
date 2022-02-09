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
    public class TituloApiOmie : ITituloApiOmie
    {

        private readonly OmieContext dbContext;
        private readonly IMapper _mapper;
        private readonly MaximaIntegracao apiMaxima;
        public TituloApiOmie(OmieContext context, IMapper mapper, MaximaIntegracao maximaIntegracao)
        {
            dbContext = context;
            _mapper = mapper;
            this.apiMaxima = maximaIntegracao;
        }

        public async Task EnviarTitulos(CancellationToken token, bool isCargaInicial = false)
        {
            LogApi log = new("Titulos");

            try
            {
                var titulosDb = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.TITULOS)
                    .AsNoTracking().ToListAsync();

                var filialDb = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.FILIAIS).FirstOrDefaultAsync();

                var processados = new List<string>();
                var cancelado = new List<string>();
                var pagina = 1;
                long totalPaginas;

                ParamN paramN = GetParamDtInicioBusca(isCargaInicial);

                do
                {
                    var listaIncluir = new List<TitulosMaxima>();
                    var listaAlterar = new List<TitulosMaxima>();

                    paramN.Pagina = pagina;

                    var request = new RequestApiOmiePorN
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "PesquisarLancamentos",
                        ParamsN = new List<ParamN>() { paramN }
                    };

                    var resultApi = await ApiUtilsMaxima.RequisicaoAsync<ResponsePesquisaTituloOmie, RequestApiOmiePorN>(PesquisaTituloOmie.UrlApi, HttpMethod.Post, request);

                    totalPaginas = resultApi.TotalDePaginas;

                    foreach (var tituloOmie in resultApi.Titulos)
                    {
                        var mapTitulo = _mapper.Map<TitulosMaxima>(tituloOmie);
                        mapTitulo.CodigoFilial = filialDb.Chave;

                        if (String.IsNullOrEmpty(mapTitulo.NumeroTransacaoVenda) || mapTitulo.NumeroTransacaoVenda.Length > 10 || String.IsNullOrEmpty(tituloOmie.cabecTitulo.cNumDocFiscal))
                            continue;

                        var chaveDb = tituloOmie.cabecTitulo.nCodTitulo.ToString();

                        if (titulosDb.Any() && titulosDb.Any(x => x.Chave == chaveDb && x.Valor != mapTitulo.Hash))
                        {
                            listaAlterar.Add(mapTitulo);
                        }
                        else if (!titulosDb.Any(x => x.Chave == chaveDb))
                        {
                            listaIncluir.Add(mapTitulo);
                        }

                        if (mapTitulo.Status.Equals("C"))
                            cancelado.Add(chaveDb);
                    }

                    if (listaIncluir.Any())
                    {
                        ResponseApiMaxima<TitulosMaxima> retornoApiMaxima = await apiMaxima.IncluirTitulos(listaIncluir);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(c => c.NumeroDuplicata.ToString()));
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                ControleDadosModel tituloModel = new()
                                {
                                    Tabela = ControleDadosEnum.TITULOS,
                                    Chave = item.NumeroDuplicata.ToString(),
                                    Valor = item.Hash
                                };
                                dbContext.ControleDadosModels.Add(tituloModel);
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
                        ResponseApiMaxima<TitulosMaxima> retornoApiMaxima = await apiMaxima.AlterarTitulos(listaAlterar);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(c => c.NumeroDuplicata.ToString()));
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                var tituloModel = titulosDb.Where(c => c.Chave == item.NumeroDuplicata.ToString()).FirstOrDefault();
                                if (tituloModel != null)
                                {
                                    tituloModel.Valor = item.Hash;
                                    dbContext.Update(tituloModel);
                                }
                            }
                            await dbContext.SaveChangesAsync();
                            log.AlterarOk(pagina, totalPaginas, listaAlterar.Count);
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

                var tituloList = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(x => x.Tabela == ControleDadosEnum.TITULOS)
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(tituloList)
                    .ToList();

                var titulosRemove = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave) && f.Tabela == ControleDadosEnum.TITULOS)
                    .ToListAsync();

                if (cancelado.Any())
                {
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarTitulos(cancelado.ToArray());
                    log.InserirOk(1, 1, cancelado.Count());
                }

                var dtInicioBuscaTituloAtulidar = await dbContext.Parametros
                                    .Where(x => x.Nome.Contains(ConstantesEnum.DtInicioBuscaTitulo))
                                    .FirstOrDefaultAsync();

                if (dtInicioBuscaTituloAtulidar == null)
                {
                    ParametroModel configuracao = new()
                    {
                        Nome = ConstantesEnum.DtInicioBuscaTitulo,
                        Valor = DateTime.Now.ToString("dd/MM/yyyy")
                    };
                    dbContext.Parametros.Add(configuracao);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    dtInicioBuscaTituloAtulidar.Valor = DateTime.Now.ToString("dd/MM/yyyy");
                    dbContext.Update(dtInicioBuscaTituloAtulidar);
                    await dbContext.SaveChangesAsync();
                }

                if (excluidos.Any())
                {
                    dbContext.ControleDadosModels.RemoveRange(titulosRemove);

                    var listaExclusao = titulosRemove.Select(x => x.Valor);
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarTitulos(listaExclusao.ToArray());
                    if (retornoApiMaxima.Sucesso)
                    {
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

        private ParamN GetParamDtInicioBusca(bool isCargaInicial)
        {
            var paramRetorno = new ParamN();

            if (isCargaInicial)
            {
                paramRetorno = new ParamN()
                {
                    Natureza = "R",
                    DtInicio = DateTime.Now.AddDays(-180).ToString("dd/MM/yyyy"),
                    DtAte = DateTime.Now.ToString("dd/MM/yyyy")
                };
            }
            else
            {
                var dtInicioBuscaTitulo = dbContext.Parametros
                    .AsNoTracking()
                    .Where(x => x.Nome.Contains(ConstantesEnum.DtInicioBuscaTitulo))
                    .First();

                paramRetorno = new ParamN()
                {
                    Natureza = "R",
                    DtAlteradoInicio = dtInicioBuscaTitulo?.Valor ?? DateTime.Now.ToString("dd/MM/yyyy")
                };
            }
            return paramRetorno;
        }
    }
}
