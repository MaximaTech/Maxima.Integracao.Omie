using Abstractions;
using Domain.DTO.FrontEnd;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Data.Models;
using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Cliente.Omie.Domain.Enums;
using Maxima.Cliente.Omie.Domain.Interfaces.Api;
using Maxima.Net.SDK.Integracao.Api;
using Maxima.Net.SDK.Integracao.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maxima.Cliente.Omie.Controllers
{
    public class OmieController : Controller
    {
        private readonly IServiceProvider _services;
        private readonly OmieContext dbContext;
        private readonly MaximaIntegracao api;
        private readonly IConfiguracao _configuracao;
        private readonly IContaCorrenteApiOmie _contaCorrente;
        private readonly IMeiosDePagamentosApiOmie _meioDePagamento;
        private readonly ICategoriaApiOmie _categoria;
        private readonly ILocalEstoqueApiOmie _localEstoque;
        private readonly IEtapaPedidoApiOmie _etapaPedido;


        public OmieController(
            OmieContext context,
            IServiceProvider services,
            MaximaIntegracao maximaIntegracao,
            IConfiguracao configuracao,
            IContaCorrenteApiOmie contaCorrente,
            IMeiosDePagamentosApiOmie meioDePagamento,
            ICategoriaApiOmie categoria,
            ILocalEstoqueApiOmie localEstoque,
            IEtapaPedidoApiOmie etapa
            )
        {
            _services = services;
            dbContext = context;
            this.api = maximaIntegracao;
            _configuracao = configuracao;
            _contaCorrente = contaCorrente;
            _meioDePagamento = meioDePagamento;
            _categoria = categoria;
            _localEstoque = localEstoque;
            _etapaPedido = etapa;
        }

        [HttpGet]
        public IActionResult Omie()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Omie([Bind("AppKey,AppSecret"), FromForm] ConfiguracaoOmieDTO configuracaoOmie)
        {
            try
            {
                var autenticado = await _configuracao.AutenticarOmie(configuracaoOmie.AppKey, configuracaoOmie.AppSecret);
                if (autenticado)
                {
                    var configAppKey = await _configuracao.Buscar(nome: ConstantesEnum.AppKeyOmie);
                    if (configAppKey == null)
                    {
                        ParametroModel appKey = new() { Nome = ConstantesEnum.AppKeyOmie, Valor = configuracaoOmie.AppKey };
                        ParametroModel appSecret = new() { Nome = ConstantesEnum.AppSecretOmie, Valor = configuracaoOmie.AppSecret };
                        dbContext.Parametros.Add(appKey);
                        dbContext.Parametros.Add(appSecret);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        configAppKey.Valor = configuracaoOmie.AppKey;

                        var configAppSecret = await _configuracao.Buscar(nome: ConstantesEnum.AppSecretOmie);
                        configAppSecret.Valor = configuracaoOmie.AppSecret;

                        await _configuracao.Alterar(configAppKey);
                        await _configuracao.Alterar(configAppSecret);
                    }
                    return RedirectToAction("OmieEtapa2", "Omie");
                }
                else
                {
                    ModelState.AddModelError("", "Usuário ou senha inválidos, Não foi possivel logar");
                    return View();
                }


            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Não foi possivel selecionar o ERP: " + ex.Message);
                return View();
            }
        }


        [HttpGet]
        public async Task<IActionResult> OmieEtapa2()
        {
            try
            {
                var configFrete = dbContext.Parametros.Where(c => c.Nome == ConstantesEnum.ValorFreteOmie).FirstOrDefault();
                var configBoleto = dbContext.Parametros.Where(c => c.Nome == ConstantesEnum.ValorParcelaBoletoOmie).FirstOrDefault();
                ViewBag.ValorFrete = configFrete != null ? configFrete?.Valor : "0";
                ViewBag.ValorParcelaBoleto = configBoleto != null ? configBoleto?.Valor : "0";

                var configCategoria = await _configuracao.Buscar(nome: ConstantesEnum.CategoriaOmie);
                if (configCategoria == null)
                {
                    ParametroModel categoriaModel = new() { Nome = ConstantesEnum.CategoriaOmie };
                    ParametroModel estoqueModel = new() { Nome = ConstantesEnum.LocalEstoqueOmie };
                    ParametroModel contaCorrenteModel = new() { Nome = ConstantesEnum.ContaCorrenteOmie };
                    dbContext.Parametros.Add(categoriaModel);
                    dbContext.Parametros.Add(estoqueModel);
                    dbContext.SaveChanges();
                    configCategoria = await _configuracao.Buscar(nome: ConstantesEnum.CategoriaOmie);
                }
                var configLocalEstoque = await _configuracao.Buscar(nome: ConstantesEnum.LocalEstoqueOmie);
                ViewBag.CodCategoria = configCategoria;
                ViewBag.CodLocalEstoque = configLocalEstoque;

                var Categoria = await _categoria.RecuperarCategoria();
                if (!Categoria.Any())
                    ViewBag.Categoria = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("SemDados", "Sem Dados Cadastrados") };
                var itensSelectCategoria = new SelectList(Categoria, nameof(CategoriaOmie.codigo), nameof(CategoriaOmie.descricao));
                foreach (var item in itensSelectCategoria)
                {
                    if (item.Value == configCategoria.Valor)
                        item.Selected = true;
                }

                ViewBag.Categoria = itensSelectCategoria;


                var localEstoque = await _localEstoque.RecuperarLocalEstoque();
                if (!localEstoque.Any())
                    ViewBag.LocalEstoque = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("SemDados", "Sem Dados Cadastrados") };
                var itensSelectEstoque = new SelectList(localEstoque, nameof(LocalEstoqueOmie.codigo_local_estoque), nameof(LocalEstoqueOmie.descricao));
                foreach (var estoque in itensSelectEstoque)
                {
                    if (configLocalEstoque != null && estoque.Value == configLocalEstoque.Valor)
                        estoque.Selected = true;
                }
                ViewBag.LocalEstoque = itensSelectEstoque;

                List<EtapaPedidoOmie> etapaOmie = await _etapaPedido.RecuperarEtapaOmie();
                if (!etapaOmie.Any())
                    etapaOmie.Add(new EtapaPedidoOmie() { Key = "SemDados", Value = "Sem Dados Cadastrados" });

                var etapaMaxima = ApiUtilsMaxima.RecuperarEtapaMaxima();
                foreach (var item in etapaOmie)
                {
                    var selectedValue = dbContext.ControleDadosModels.FirstOrDefault(x => x.Tabela == ControleDadosEnum.ETAPAPEDIDO && x.Chave == item.Key);
                    item.EtapasMaxima = new SelectList(etapaMaxima, "Key", "Value", selectedValue?.Valor);
                }
                ViewBag.EtapaOmie = etapaOmie;

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OmieEtapa2([Bind("CodCategoria,CodLocalEstoque,CodEtapaOmie,CodEtapaMaxima,ValorParcelaBoleto,ValorFrete"), FromForm] ConfiguracaoOmieEtapa2DTO configuracaoOmie)
        {
            try
            {

                var configCategoria = await _configuracao.Buscar(nome: ConstantesEnum.CategoriaOmie);
                configCategoria.Valor = configuracaoOmie.CodCategoria;
                await _configuracao.Alterar(configCategoria);

                var configLocalEstoque = await _configuracao.Buscar(nome: ConstantesEnum.LocalEstoqueOmie);
                if (configLocalEstoque == null)
                {
                    configLocalEstoque = new ParametroModel
                    {
                        Nome = ConstantesEnum.LocalEstoqueOmie,
                        Valor = configuracaoOmie.CodLocalEstoque
                    };
                    dbContext.Parametros.Add(configLocalEstoque);
                    dbContext.SaveChanges();
                }
                configLocalEstoque.Valor = configuracaoOmie.CodLocalEstoque;
                await _configuracao.Alterar(configLocalEstoque);

                if (!string.IsNullOrEmpty(configuracaoOmie.ValorParcelaBoleto))
                {

                    var configValorParcelaBoleto = await _configuracao.Buscar(nome: ConstantesEnum.ValorParcelaBoletoOmie);
                    if (configValorParcelaBoleto == null)
                    {
                        configValorParcelaBoleto = new ParametroModel();
                        configValorParcelaBoleto.Nome = ConstantesEnum.ValorParcelaBoletoOmie;
                        configValorParcelaBoleto.Valor = configuracaoOmie.ValorParcelaBoleto.Replace(".", ",");
                        dbContext.Parametros.Add(configValorParcelaBoleto);
                        await dbContext.SaveChangesAsync();
                    }
                    configValorParcelaBoleto.Valor = configuracaoOmie.ValorParcelaBoleto.Replace(".", ",");
                    await _configuracao.Alterar(configValorParcelaBoleto);
                }

                if (!string.IsNullOrEmpty(configuracaoOmie.ValorFrete))
                {
                    var configValorFrete = await _configuracao.Buscar(nome: ConstantesEnum.ValorFreteOmie);
                    if (configValorFrete == null)
                    {
                        configValorFrete = new ParametroModel();
                        configValorFrete.Nome = ConstantesEnum.ValorFreteOmie;
                        configValorFrete.Valor = configuracaoOmie.ValorFrete.Replace(".", ",");
                        dbContext.Parametros.Add(configValorFrete);
                        await dbContext.SaveChangesAsync();
                    }
                    configValorFrete.Valor = configuracaoOmie.ValorFrete.Replace(".", ",");
                    await _configuracao.Alterar(configValorFrete);
                }

                for (int i = 0; i < configuracaoOmie.CodEtapaMaxima.Count; i++)
                {
                    ControleDadosModel etapasComparacaoModel = new()
                    {
                        Tabela = ControleDadosEnum.ETAPAPEDIDO,
                        Chave = configuracaoOmie.CodEtapaOmie[i],
                        Valor = configuracaoOmie.CodEtapaMaxima[i]
                    };
                    if (dbContext.ControleDadosModels.Any(e => e.Tabela == ControleDadosEnum.ETAPAPEDIDO && e.Chave == etapasComparacaoModel.Chave))
                        dbContext.ControleDadosModels.Update(etapasComparacaoModel);
                    else
                        dbContext.ControleDadosModels.Add(etapasComparacaoModel);

                }

                await dbContext.SaveChangesAsync();

                return RedirectToAction("OmieEtapa3", "Omie");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Não foi possivel selecionar o ERP: " + ex.Message);
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> OmieEtapa3()
        {
            try
            {
                var configContatorrente = await _configuracao.Buscar(nome: ConstantesEnum.ContaCorrenteOmie);
                ViewBag.CodContaCorrente = configContatorrente;

                List<MeiosDePagamentoOmie> meioPagamentos = await _meioDePagamento.RecuperarMeiosdePagamentos();
                if (!meioPagamentos.Any())
                    meioPagamentos.Add(new MeiosDePagamentoOmie() { Key = "SemDados", Value = "Sem Dados Cadastrados" });

                var contaCorrente = _contaCorrente.RecuperarContaCorrente();
                foreach (var item in meioPagamentos)
                {
                    var selectValue = dbContext.ControleDadosModels.FirstOrDefault(x => x.Tabela == ControleDadosEnum.MEIOPAGAMENTOCOMPARACAO && x.Chave == item.Key);
                    item.contasCorrentes = new SelectList(await contaCorrente, "Key", "Value", selectValue?.Valor);
                }
                ViewBag.MeiosDePagamento = meioPagamentos;
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OmieEtapa3([Bind("ContaCorrente,MeiosDePagamento"), FromForm] ConfiguracaoOmieEtapa3DTO configuracaoOmie)
        {
            try
            {

                if (configuracaoOmie.ContaCorrente.Any())
                    for (int i = 0; i < configuracaoOmie.MeiosDePagamento.Count; i++)
                    {
                        ControleDadosModel meioPagamentoComparacaoModel = new()
                        {
                            Tabela = ControleDadosEnum.MEIOPAGAMENTOCOMPARACAO,
                            Chave = configuracaoOmie.MeiosDePagamento[i],
                            Valor = configuracaoOmie.ContaCorrente[i]
                        };
                        if (dbContext.ControleDadosModels.Any(e => e.Tabela == ControleDadosEnum.MEIOPAGAMENTOCOMPARACAO && e.Chave == meioPagamentoComparacaoModel.Chave))
                            dbContext.ControleDadosModels.Update(meioPagamentoComparacaoModel);
                        else
                            dbContext.ControleDadosModels.Add(meioPagamentoComparacaoModel);
                    }

                await dbContext.SaveChangesAsync();

                await _configuracao.ProximaEtapaConfig();

                return RedirectToAction("Index", "Configuracao");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Não foi possivel selecionar o meio de pagamento: " + ex.Message);
                return View();
            }
        }


    }
}
