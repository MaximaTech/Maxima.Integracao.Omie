using Abstractions;
using Domain.DTO.FrontEnd;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Data.Models;
using Maxima.Cliente.Omie.Domain.Enums;
using Maxima.Cliente.Omie.Domain.Interfaces;
using Maxima.Cliente.Omie.Domain.Work;
using Maxima.Net.SDK.Integracao.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Maxima.Cliente.Omie.Controllers
{
    public class ConfiguracaoController : Controller
    {
        private readonly IServiceProvider _services;
        private readonly OmieContext dbContext;
        private readonly MaximaIntegracao apiMaxima;
        private readonly IConfiguracao _configuracao;
        private readonly IJobs _Jobs;
        private readonly WorkPubSubMaxima workPubSubMaxima;


        public ConfiguracaoController(OmieContext context, IServiceProvider services, MaximaIntegracao maximaIntegracao, IConfiguracao configuracao, IJobs jobs, WorkPubSubMaxima workPubSubMaxima)
        {
            _services = services;
            dbContext = context;
            this.apiMaxima = maximaIntegracao;
            _configuracao = configuracao;
            _Jobs = jobs;
            this.workPubSubMaxima = workPubSubMaxima;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var etapa = await dbContext.Parametros
                    .Where(x => x.Nome.Contains(ConstantesEnum.EtapaConfig))
                    .Select(x => x.Valor)
                    .FirstOrDefaultAsync();

                _ = int.TryParse(etapa, out var valorEnum);

                return (EnumEtapasConfig)valorEnum switch
                {
                    EnumEtapasConfig.ERP => RedirectToAction("Omie", "Omie"),
                    //EnumEtapasConfig.AssinaturaPubSub => RedirectToAction("AssinaturaPubSub", "ConfiguracaoPubSub"),
                    EnumEtapasConfig.CronJob => RedirectToAction("Jobs", "Jobs"),
                    EnumEtapasConfig.CargaInicial => RedirectToAction("CargaInicial", "CargaInicial"),
                    EnumEtapasConfig.CargaInicialErro => RedirectToAction(nameof(CargaInicialErro)),
                    EnumEtapasConfig.Finalizado => RedirectToAction(nameof(Finalizado)),
                    _ => RedirectToAction(nameof(Maxima)),
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public IActionResult Maxima()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ReIniciar()
        {
            //Process.Start(Constantes.CaminhoAtualizador + "Atualizador.exe", "-R");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Maxima([Bind("Login,Senha"), FromForm] ConfiguracaoMaximaDTO configuracaoMaxima)
        {
            try
            {
                var logado = apiMaxima.RealizarLogin(configuracaoMaxima.Login, configuracaoMaxima.Senha);

                if (logado)
                {
                    var configLogin = await _configuracao.Buscar(nome: ConstantesEnum.LoginMaxima);
                    if (configLogin == null)
                    {
                        ParametroModel login = new() { Nome = ConstantesEnum.LoginMaxima, Valor = configuracaoMaxima.Login };
                        ParametroModel senha = new() { Nome = ConstantesEnum.SenhaMaxima, Valor = configuracaoMaxima.Senha };
                        dbContext.Parametros.Add(login);
                        dbContext.Parametros.Add(senha);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        configLogin.Valor = configuracaoMaxima.Login;

                        var configSenha = await _configuracao.Buscar(nome: ConstantesEnum.SenhaMaxima);
                        configSenha.Valor = configuracaoMaxima.Senha;

                        await _configuracao.Alterar(configLogin);
                        await _configuracao.Alterar(configSenha);
                    }
                    await _configuracao.ProximaEtapaConfig();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Usuário ou senha inválidos, Não foi possivel logar: ");
                    return View(nameof(Maxima));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Não foi possivel logar: " + ex.Message);
                return View(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CargaInicial()
        {
            try
            {
                _configuracao.VoltarCargaInicial();

                return RedirectToAction("Index", "Configuracao");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Não foi realizado a Carga Inicial completa: " + ex.Message);
                return View();
            }

        }


        [HttpGet]
        public IActionResult Finalizado()
        {
            ViewBag.EtapaFinalizador = _configuracao.EtapaAtual();

            var login = dbContext.Parametros.Where(p => p.Nome == ConstantesEnum.LoginMaxima).FirstOrDefault()?.Valor ?? "";
            var senha = dbContext.Parametros.Where(p => p.Nome == ConstantesEnum.SenhaMaxima).FirstOrDefault()?.Valor ?? "";

            apiMaxima.RealizarLogin(login, senha);
            workPubSubMaxima.IniciarOuvintes();

            _Jobs.RecuperarJobsERPs();
            return View();
        }

        [HttpGet]
        public IActionResult CargaInicialErro()
        {
            ViewBag.EtapaCargaInicialErro = _configuracao.EtapaAtual();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResetarConfiguracao()
        {
            try
            {
                var configLogin = await _configuracao.Buscar(nome: ConstantesEnum.EtapaConfig);
                configLogin.Valor = ((int)EnumEtapasConfig.Maxima).ToString();
                await _configuracao.Alterar(configLogin);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
