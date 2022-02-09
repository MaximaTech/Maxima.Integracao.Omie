using Abstractions;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Domain.Enums;
using Maxima.Cliente.Omie.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Maxima.Cliente.Omie.Controllers
{
    public class CargaInicialController : Controller
    {
        private readonly IServiceProvider _services;
        private readonly OmieContext dbContext;
        private readonly IConfiguracao _configuracao;
        private readonly IJobs _Jobs;


        public CargaInicialController(
            OmieContext context,
            IServiceProvider services,
            IConfiguracao configuracao,
            IJobs jobs
            )
        {
            _services = services;
            dbContext = context;
            _configuracao = configuracao;
            _Jobs = jobs;

        }


        [HttpGet]
        public async Task<IActionResult> CargaInicial()
        {
            var etapa = (await _configuracao.Buscar(nome: ConstantesEnum.EtapaConfig)).Valor;
            _ = int.TryParse(etapa, out var valorEnum);
            if (valorEnum == (int)EnumEtapasConfig.Finalizado)
                return RedirectToAction("Index", "Configuracao");

            var config = await _configuracao.Buscar(nome: ConstantesEnum.CargaInicial);
            if (string.IsNullOrEmpty(config?.Valor))
            {
                _Jobs.CargaInicial();
                if (config == null)
                {

                    dbContext.Parametros.Add(new Data.Models.ParametroModel()
                    {
                        Nome = ConstantesEnum.CargaInicial,
                        Valor = true.ToString()
                    });
                }
                else
                {
                    config.Valor = true.ToString();
                    dbContext.Parametros.Update(config);
                }

                await dbContext.SaveChangesAsync();

            }
            var configCargaFinalizada = await _configuracao.Buscar(nome: ConstantesEnum.CargaInicialFinalizada);
            if (!string.IsNullOrEmpty(configCargaFinalizada?.Valor) && configCargaFinalizada.Valor.ToUpper().Equals("TRUE"))
            {
                return RedirectToAction("Index", "Configuracao");
            }

            var configCargaErro = await _configuracao.Buscar(nome: ConstantesEnum.CargaInicialErro);
            if (!string.IsNullOrEmpty(configCargaErro?.Valor) && configCargaErro.Valor.ToUpper().Equals("TRUE"))
            {
                return RedirectToAction("Index", "Configuracao");
            }

            return View();
        }
    }
}
