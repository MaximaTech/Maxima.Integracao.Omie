using Abstractions;
using Domain.DTO.FrontEnd;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Data.Models;
using Maxima.Cliente.Omie.Domain.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maxima.Cliente.Omie.Controllers
{
    public class JobsController : Controller
    {
        private readonly IServiceProvider _services;
        private readonly OmieContext dbContext;

        private readonly IConfiguracao _configuracao;


        public JobsController(OmieContext context, IServiceProvider services, IConfiguracao configuracao)
        {
            _services = services;
            dbContext = context;
            _configuracao = configuracao;
        }

        [HttpGet]
        public IActionResult Jobs()
        {

            Dictionary<string, string> listasJobs = JobsCronOmie.GetJobsCron();

            var jobs = new List<JobsCronDTO>();

            foreach (var item in listasJobs)
            {
                JobsCronDTO job = new()
                {
                    Descricao = item.Value,
                    Job = item.Key,
                    Cron = dbContext.JobsModels
                        .Where(x => x.Nome == item.Key)
                        .Select(x => x.Horario)
                        .FirstOrDefault() ?? "23:00"
                };

                jobs.Add(job);
            }

            ViewBag.ListaJobs = jobs;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JobsCronSave([FromForm] List<JobsCronDTO> jobs)
        {
            try
            {
                foreach (var job in jobs)
                {
                    var jobBanco = await dbContext.JobsModels
                            .Where(x => x.Nome == job.Job)
                            .FirstOrDefaultAsync();
                    var separado = job.Cron.Split(":");

                    if (jobBanco == null)
                    {
                        JobsModel jobsModel = new JobsModel();
                        jobsModel.Nome = job.Job;
                        jobsModel.Horario = job.Cron;
                        jobsModel.Valor = _configuracao.ConvertToCron(separado[0], separado[1]);
                        dbContext.JobsModels.Add(jobsModel);
                    }
                    else
                    {

                        jobBanco.Horario = job.Cron;
                        jobBanco.Valor = _configuracao.ConvertToCron(separado[0], separado[1]);
                    }
                }
                await dbContext.SaveChangesAsync();
                await _configuracao.ProximaEtapaConfig();
                JobsCronOmie.GetJobsCron();

                return RedirectToAction("Index", "Configuracao");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Não foi Salvar o novo horario: " + ex.Message);
                return View();
            }
        }
    }
}
