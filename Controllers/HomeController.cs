using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Domain.Enums;
using Maxima.Cliente.Omie.Models;
using Maxima.Net.SDK.Integracao.Api;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace Maxima.Cliente.Omie.Controllers
{
    public class HomeController : Controller
    {

        public HomeController(MaximaIntegracao api, OmieContext dbContext)
        {
            var usuario = dbContext.Parametros.Where(p => p.Nome == ConstantesEnum.LoginMaxima).FirstOrDefault()?.Valor ?? "";
            var senha = dbContext.Parametros.Where(p => p.Nome == ConstantesEnum.SenhaMaxima).FirstOrDefault()?.Valor ?? "";
            api.RealizarLogin(usuario, senha);

        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Configuracao");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
