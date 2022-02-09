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
    public class VendedorApiOmie : IVendedorApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper _mapper;
        private readonly MaximaIntegracao apiMaxima;

        public VendedorApiOmie(OmieContext context, IMapper mapper, MaximaIntegracao api)
        {
            dbContext = context;
            _mapper = mapper;
            apiMaxima = api;
        }

        public async Task EnviarVendedores(CancellationToken token)
        {
            LogApi log = new("Vendedores");

            try
            {
                var vendedorDb = await dbContext.ControleDadosModels
                    .Where(c => c.Tabela == ControleDadosEnum.VENDEDORES)
                    .AsNoTracking()
                    .ToListAsync();

                var codFilial = dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.FILIAIS).FirstOrDefault();
                var listaIncluir = new List<VendedorMaxima>();
                var listaAlterar = new List<VendedorMaxima>();
                var processados = new List<string>();
                var pagina = 1;
                long totalPaginas;
                long countOmie;

                do
                {
                    var request = new RequestOmie
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "ListarVendedores",
                        Params = new List<Param>() { new Param() { Pagina = pagina } }
                    };

                    var result = await ApiUtilsMaxima.RequisicaoAsync<ResponseVendedorOmie, RequestOmie>(VendedorOmie.UrlApi, HttpMethod.Post, request);

                    totalPaginas = result.TotalDePaginas;
                    countOmie = result.TotalDeRegistros;
                    result.Vendedores.RemoveAll(v => v.inativo == "S");

                    foreach (var obj in result.Vendedores)
                    {
                        var mapVendedor = _mapper.Map<VendedorMaxima>(obj);
                        mapVendedor.Codfilial = codFilial?.Chave ?? null;

                        if (vendedorDb.Any(x => x.Chave == mapVendedor.CodigoVendedor && x.Valor != mapVendedor.Hash))
                        {
                            listaAlterar.Add(mapVendedor);
                        }
                        else if (!vendedorDb.Any(x => x.Chave == mapVendedor.CodigoVendedor))
                        {
                            listaIncluir.Add(mapVendedor);
                        }
                    }

                    if (listaIncluir.Any())
                    {
                        ResponseApiMaxima<VendedorMaxima> retornoApiMaxima = await apiMaxima.IncluirVendedor(listaIncluir);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoVendedor.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                ControleDadosModel vendedorModel = new()
                                {
                                    Tabela = ControleDadosEnum.VENDEDORES,
                                    Chave = item.CodigoVendedor,
                                    Valor = item.Hash
                                };
                                dbContext.ControleDadosModels.Add(vendedorModel);
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
                        ResponseApiMaxima<VendedorMaxima> retornoApiMaxima = await apiMaxima.AlterarVendedor(listaAlterar);
                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoVendedor.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {

                                var vendedorModel = vendedorDb.Where(c => c.Chave == item.CodigoVendedor).FirstOrDefault();
                                if (vendedorModel != null)
                                {
                                    vendedorModel.Valor = item.Hash;
                                    dbContext.Update(vendedorModel);
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

                var vendedorList = await dbContext.ControleDadosModels
                    .Where(c => c.Tabela == ControleDadosEnum.VENDEDORES)
                    .AsNoTracking()
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(vendedorList)
                    .ToList();

                var vendedorRemove = await dbContext.ControleDadosModels
                    .Where(c => c.Tabela == ControleDadosEnum.VENDEDORES)
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave))
                    .ToListAsync();

                if (excluidos.Any())
                {
                    var listaExclusao = vendedorRemove.Select(x => x.Valor);
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarVendedor(listaExclusao.ToArray());

                    if (retornoApiMaxima.Sucesso)
                    {
                        dbContext.ControleDadosModels.RemoveRange(vendedorRemove);
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
