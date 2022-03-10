using System;
using System.Linq;
using Maxima.Cliente.Omie.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Maxima.Cliente.Omie.Data
{
    public class OmieContext : DbContext
    {
        public OmieContext(DbContextOptions<OmieContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OmieContext).Assembly);
            var models = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties()).Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));
            foreach (var property in models)
            {
                property.SetColumnType("timestamp without time zone");
            }

        }
        public DbSet<ParametroModel> Parametros { get; set; }
        public DbSet<ControleDadosModel> ControleDadosModels { get; set; }
        public DbSet<JobsModel> JobsModels { get; set; }
    }
}