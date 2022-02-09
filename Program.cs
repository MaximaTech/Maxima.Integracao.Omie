using System;
using System.Linq;
using System.Threading.Tasks;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Domain.Enums;
using Maxima.Cliente.Omie.Domain.Work;
using Maxima.Net.SDK.Integracao.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Maxima.Cliente.Omie
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetService<OmieContext>();
                var apiMaxima = services.GetService<MaximaIntegracao>();
                var workPubSubMaxima = services.GetService<WorkPubSubMaxima>();
                dbContext.Database.SetCommandTimeout((int)TimeSpan.FromMinutes(5).TotalSeconds);
                await dbContext.Database.MigrateAsync();

                var usuario = dbContext.Parametros.Where(p => p.Nome == ConstantesEnum.LoginMaxima).FirstOrDefault()?.Valor ?? "";
                var senha = dbContext.Parametros.Where(p => p.Nome == ConstantesEnum.SenhaMaxima).FirstOrDefault()?.Valor ?? "";
                apiMaxima.RealizarLogin(usuario, senha);
                workPubSubMaxima.IniciarOuvintes();
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
