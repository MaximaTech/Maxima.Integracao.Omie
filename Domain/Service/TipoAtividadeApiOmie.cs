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
    public class TipoAtividadeApiOmie : ITipoAtividadeApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;
        public TipoAtividadeApiOmie(OmieContext context, IMapper mappe, MaximaIntegracao maximaIntegracao)
        {
            dbContext = context;
            mapper = mappe;
            apiMaxima = maximaIntegracao;
        }

        public async Task EnviarTipoAtividade(CancellationToken token)
        {
            LogApi log = new("Ramos Atividades");

            try
            {
                var atividadesbd = await dbContext.ControleDadosModels
                    .Where(c => c.Tabela == ControleDadosEnum.TIPOATIVIDADE)
                    .AsNoTracking()
                    .ToListAsync();

                var listaIncluir = new List<RamoAtividadeMaxima>();
                var listaAlterar = new List<RamoAtividadeMaxima>();
                var processados = new List<string>();

                var request = new RequestTipoAtividadeOmie
                {
                    AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                    AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                    Call = "ListarTipoAtiv",
                    Params = new List<ParamTipoAtividade>() { new ParamTipoAtividade() }
                };

                var result = await ApiUtilsMaxima.RequisicaoAsync<ResponseTipoAtividadeOmie, RequestTipoAtividadeOmie>(TipoAtividadeOmie.UrlApi, HttpMethod.Post, request);

                foreach (var obj in result.TiposAtividades)
                {
                    var mapAtividades = mapper.Map<RamoAtividadeMaxima>(obj);

                    if (atividadesbd.Any(x => x.Chave == mapAtividades.CodigoRamoAtividadeEconomica && x.Valor != mapAtividades.Hash))
                    {
                        listaAlterar.Add(mapAtividades);
                    }
                    else if (!atividadesbd.Any(x => x.Chave == mapAtividades.CodigoRamoAtividadeEconomica))
                    {
                        listaIncluir.Add(mapAtividades);
                    }
                }

                if (listaIncluir.Any())
                {
                    ResponseApiMaxima<RamoAtividadeMaxima> retornoApiMaxima = await apiMaxima.IncluirAtividade(listaIncluir);
                    if (retornoApiMaxima.Sucesso)
                    {
                        processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoRamoAtividadeEconomica).ToList());
                        foreach (var item in retornoApiMaxima.ItensInserido)
                        {
                            ControleDadosModel model = new()
                            {
                                Tabela = ControleDadosEnum.TIPOATIVIDADE,
                                Chave = item.CodigoRamoAtividadeEconomica,
                                Valor = item.Hash
                            };
                            dbContext.ControleDadosModels.Add(model);
                        }
                        await dbContext.SaveChangesAsync();
                        log.InserirOk(1, 1, retornoApiMaxima.ItensInserido.Count());

                        if (retornoApiMaxima.ErrosValidacao.Any())
                            log.InserirErro(1, 1, retornoApiMaxima.TotalItensNaoInserido, retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                    else
                    {
                        log.InserirErro(1, 1, listaIncluir.Count(), retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                }

                if (listaAlterar.Any())
                {
                    ResponseApiMaxima<RamoAtividadeMaxima> retornoApiMaxima = await apiMaxima.AlterarAtividade(listaAlterar);
                    if (retornoApiMaxima.Sucesso)
                    {
                        processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoRamoAtividadeEconomica).ToList());
                        foreach (var item in retornoApiMaxima.ItensInserido)
                        {
                            var modelAtividades = atividadesbd.Where(c => c.Chave == item.CodigoRamoAtividadeEconomica).FirstOrDefault();
                            if (modelAtividades != null)
                            {
                                modelAtividades.Valor = item.Hash;
                                dbContext.Update(modelAtividades);
                            }
                        }
                        await dbContext.SaveChangesAsync();
                        log.InserirOk(1, 1, retornoApiMaxima.ItensInserido.Count());

                        if (retornoApiMaxima.ErrosValidacao.Any())
                            log.InserirErro(1, 1, retornoApiMaxima.TotalItensNaoInserido, retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                    else
                    {
                        log.AlterarErro(1, 1, listaAlterar.Count, retornoApiMaxima.ErrosValidacaoFormatado);
                    }
                }

                var tipoAtividadeList = await dbContext.ControleDadosModels
                    .Where(c => c.Tabela == ControleDadosEnum.TIPOATIVIDADE)
                    .AsNoTracking()
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(tipoAtividadeList)
                    .ToList();

                var tipoAtividadeRemove = await dbContext.ControleDadosModels
                    .Where(c => c.Tabela == ControleDadosEnum.TIPOATIVIDADE)
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave))
                    .ToListAsync();

                if (excluidos.Any())
                {

                    var listaExclusao = tipoAtividadeRemove.Select(x => x.Valor);
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarAtividades(listaExclusao.ToArray());

                    if (retornoApiMaxima.Sucesso)
                    {
                        dbContext.ControleDadosModels.RemoveRange(tipoAtividadeRemove);
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
