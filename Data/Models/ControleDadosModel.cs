using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maxima.Cliente.Omie.Data.Models
{
    public class ControleDadosModel : IEntityTypeConfiguration<ControleDadosModel>
    {
        public int Id { get; set; }
        public string Tabela { get; set; }
        public string Chave { get; set; }
        public string Valor { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public void Configure(EntityTypeBuilder<ControleDadosModel> builder)
        {
            builder.HasKey(p => new { p.Id });
        }

        public override string ToString()
        {
            return $"{Tabela} - {Chave} - {Valor}";
        }
    }
}