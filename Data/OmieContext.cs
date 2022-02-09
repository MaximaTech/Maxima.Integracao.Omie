using System;
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
        }
        public DbSet<ParametroModel> Parametros { get; set; }
        public DbSet<ControleDadosModel> ControleDadosModels { get; set; }
        public DbSet<JobsModel> JobsModels { get; set; }
    }
}