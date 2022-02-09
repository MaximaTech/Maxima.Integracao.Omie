using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
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
using Maxima.Net.SDK.Integracao.Dto.Pedido;
using Maxima.Net.SDK.Integracao.Entidades;
using Maxima.Net.SDK.Integracao.Utils;
using Microsoft.EntityFrameworkCore;

namespace Maxima.Cliente.Omie.Domain.Service
{
    public class ClienteApiOmie : IClienteApiOmie
    {
        private readonly OmieContext dbContext;
        private readonly IMapper mapper;
        private readonly MaximaIntegracao apiMaxima;
        public int CountMaxima { get; set; }

        public ClienteApiOmie(OmieContext context, IMapper mappe, MaximaIntegracao maximaIntegracao)
        {
            dbContext = context;
            mapper = mappe;
            this.apiMaxima = maximaIntegracao;
        }

        public async Task ReceberClientes(CancellationToken token)
        {
            LogApi log = new("Clientes Cadastro");

            try
            {
                var clientesMaxima = await apiMaxima.GetClientes();

                foreach (var clienteMaxima in clientesMaxima)
                {
                    var mapCliente = mapper.Map<ClienteOmie>(clienteMaxima);

                    var request = new RequestEnvioCliente
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "IncluirCliente",
                        param = new List<ClienteOmie>() { mapCliente }

                    };

                    var result = await ApiUtilsMaxima.RequisicaoAsync<ResponseClienteCadastroOmie, RequestEnvioCliente>(ClienteOmie.UrlApi, HttpMethod.Post, request);

                    if (result.Sucesso && result.codigo_cliente_omie > 0)
                    {
                        RetornoApiMaxima retornoMaxima = await apiMaxima.AlterarStatusCliente(clienteMaxima, true, result.codigo_cliente_omie.ToString());
                        if (retornoMaxima.Sucesso)
                            Console.WriteLine("Cliente importado com sucesso");
                        else
                            System.Console.WriteLine("Houve um erro ao enviar o retorno para máxima");
                    }
                    else
                    {
                        RetornoApiMaxima retornoMaxima = await apiMaxima.AlterarStatusCliente(clienteMaxima, false, null, result.faultstring + " " + result.faultcode);
                        if (retornoMaxima.Sucesso)
                            Console.WriteLine("Cliente aguardando auteração pelo vendedor");
                        else
                            System.Console.WriteLine("Houve um erro ao enviar o retorno para máxima");

                        Console.WriteLine("Erro ao enviar o cliente de codigo:" + mapCliente.codigo_cliente_omie + " Resposta da Omie: " + result.faultstring + " " + result.faultcode);

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao enviar clientes: " + ex.Message);

            }
        }

        public async Task EnviarClientes(CancellationToken token)
        {
            LogApi log = new("Clientes");

            try
            {
                var clienteBd = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.CLIENTES).AsNoTracking().ToListAsync();
                var praca = await dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.PRACAS).AsNoTracking().ToListAsync();
                var processados = new List<string>();
                var pagina = 1;
                long totalPaginas;
                long countOmie;

                do
                {
                    var listaIncluir = new List<ClienteMaxima>();
                    var listaAlterar = new List<ClienteMaxima>();

                    var request = new RequestOmie
                    {
                        AppKey = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppKeyOmie).Valor,
                        AppSecret = dbContext.Parametros.FirstOrDefault(x => x.Nome == ConstantesEnum.AppSecretOmie).Valor,
                        Call = "ListarClientes",
                        Params = new List<Param> { new Param() { Pagina = pagina, ClientesFiltro = new ClientesFiltros() } }
                    };

                    var result = await ApiUtilsMaxima.RequisicaoAsync<ResponseClienteOmie, RequestOmie>(ClienteOmie.UrlApi, HttpMethod.Post, request);

                    totalPaginas = result.TotalDePaginas;
                    countOmie = result.TotalDeRegistros;

                    foreach (var cliente in result.Clientes)
                    {

                        var mapCliente = mapper.Map<ClienteMaxima>(cliente);

                        if (praca.Any())
                            mapCliente.CodigoPraca = praca.FirstOrDefault().Chave;

                        if (clienteBd.Any() && clienteBd.Any(x => x.Chave == mapCliente.CodigoCliente && x.Valor != mapCliente.Hash))
                        {
                            listaAlterar.Add(mapCliente);
                        }
                        else if (!clienteBd.Any(x => x.Chave == mapCliente.CodigoCliente))
                        {
                            listaIncluir.Add(mapCliente);
                        }
                    }

                    if (listaIncluir.Any())
                    {
                        ResponseApiMaxima<ClienteMaxima> retornoApiMaxima = await apiMaxima.IncluirCliente(listaIncluir);
                        if (retornoApiMaxima.Sucesso)
                        {

                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoCliente.ToString()).ToList());

                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                ControleDadosModel clienteModel = new()
                                {
                                    Tabela = ControleDadosEnum.CLIENTES,
                                    Chave = item.CodigoCliente,
                                    Valor = item.Hash
                                };

                                dbContext.ControleDadosModels.Add(clienteModel);
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
                        ResponseApiMaxima<ClienteMaxima> retornoApiMaxima = await apiMaxima.AlterarCliente(listaAlterar);

                        if (retornoApiMaxima.Sucesso)
                        {
                            processados.AddRange(retornoApiMaxima.ItensInserido.Select(x => x.CodigoCliente.ToString()).ToList());
                            foreach (var item in retornoApiMaxima.ItensInserido)
                            {
                                var clienteModel = clienteBd.Where(c => c.Chave == item.CodigoCliente).FirstOrDefault();
                                if (clienteModel != null)
                                {
                                    clienteModel.Valor = item.Hash;
                                    dbContext.Update(clienteModel);
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

                var clienteList = await dbContext.ControleDadosModels
                    .AsNoTracking()
                    .Where(a => a.Tabela == ControleDadosEnum.CLIENTES)
                    .Select(f => f.Chave)
                    .ToListAsync();

                var excluidos = processados
                    .Except(clienteList)
                    .ToList();

                var clienteRemove = dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.CLIENTES)
                    .AsNoTracking()
                    .Where(f => excluidos.Contains(f.Chave))
                    .ToList();

                if (excluidos.Any())
                {
                    var listaExclusao = clienteRemove.Select(x => x.Valor);
                    RetornoApiMaxima retornoApiMaxima = await apiMaxima.DeletarClientes(listaExclusao.ToArray());
                    if (retornoApiMaxima.Sucesso)
                    {
                        dbContext.ControleDadosModels.RemoveRange(clienteRemove);
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

