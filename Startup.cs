using Abstractions;
using AutoMapper;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Logic;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Domain.Interfaces;
using Maxima.Cliente.Omie.Domain.Interfaces.Api;
using Maxima.Cliente.Omie.Domain.Logic;
using Maxima.Cliente.Omie.Domain.Mappings;
using Maxima.Cliente.Omie.Domain.Service;
using Maxima.Cliente.Omie.Domain.Utils;
using Maxima.Cliente.Omie.Domain.Work;
using Maxima.Net.SDK.Integracao.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Maxima.Cliente.Omie
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.Combine(Directory.GetCurrentDirectory(), "chave_pub_sub_subscrib.json"));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();


            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var dbPath = $"{path}{System.IO.Path.DirectorySeparatorChar}omie.db";
            System.Console.WriteLine(dbPath);
            services.AddDbContext<OmieContext>(opt => opt.UseSqlite($"Data Source={dbPath}"));

            services.AddMaximaSdkService();

            services.AddTransient<IBancoApiOmie, BancoApiOmie>();
            services.AddTransient<ICidadeApiOmie, CidadeApiOmie>();
            services.AddTransient<IClienteApiOmie, ClienteApiOmie>();
            services.AddTransient<IDepartamentoApiOmie, DepartamentoApiOmie>();
            services.AddTransient<IEstoqueApiOmie, EstoqueApiOmie>();
            services.AddTransient<IFamiliaApiOmie, FamiliaApiOmie>();
            services.AddTransient<IFilialApiOmie, FilialApiOmie>();
            services.AddTransient<IFornecedorApiOmie, FornecedorApiOmie>();
            services.AddTransient<ITituloApiOmie, TituloApiOmie>();
            services.AddTransient<IProdutoApiOmie, ProdutoApiOmie>();
            services.AddTransient<IProdutosPorFornecedorApiOmie, ProdutosPorFornecedorApiOmie>();
            services.AddTransient<ITipoAtividadeApiOmie, TipoAtividadeApiOmie>();
            services.AddTransient<ITransportadoraApiOmie, TransportadoraApiOmie>();
            services.AddTransient<IVendedorApiOmie, VendedorApiOmie>();
            services.AddTransient<ICategoriaApiOmie, CategoriaApiOmie>();
            services.AddTransient<IContaCorrenteApiOmie, ContaCorrenteApiOmie>();
            services.AddTransient<IEtapaPedidoApiOmie, EtapaPedidoApiOmie>();
            services.AddTransient<ILocalEstoqueApiOmie, LocalEstoqueApiOmie>();
            services.AddTransient<IPrecoProdutoApiOmie, PrecoProdutoApiOmie>();
            services.AddTransient<IMeiosDePagamentosApiOmie, MeiosDePagamentoApiOmie>();
            services.AddTransient<IPracaRegiaoApiOmie, PracaRegiaoApiOmie>();
            services.AddTransient<IFormaPagamentoApiOmie, FormaPagamentoApiOmie>();

            services.AddSingleton<IPedidoEnvioApiOmie, PedidoEnvioApiOmie>();
            services.AddSingleton<IPedidoHistoricoApiOmie, PedidoHistoricoApiOmie>();
            services.AddSingleton<IPedidoStatusApiOmie, PedidoStatusApiOmie>();

            services.AddTransient<IJobs, JobsOmie>();

            services.AddTransient<IConfiguracao, Configuracao>();
            services.AddTransient<WorkPubSubMaxima>();

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage(ConfiguracaoHangfire.StorageOptions));

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 2 });
            services.AddHangfireServer();

            services.AddSingleton(AutoMapperConfig.Initialize());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IBackgroundJobClient backgroundJobs, IWebHostEnvironment env, IJobs jobs)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseHangfireDashboard("/filas", ConfiguracaoHangfire.DashboardOptions());


            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHangfireDashboard();
            });

            jobs.RecuperarJobsERPs();
        }

        public class AutoMapperConfig
        {
            public static IMapper Initialize()
            {
                var mapperConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MapAll());
                });
                return mapperConfig.CreateMapper();
            }
        }
    }
}
