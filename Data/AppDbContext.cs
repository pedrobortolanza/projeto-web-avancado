using Microsoft.EntityFrameworkCore;
using PetshopPeterson.Models;

namespace PetshopPeterson.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) {}

        public DbSet<Servico> Servico { get; set; } = null!;
        public DbSet<Agendamento> Agendamento { get; set; } = null!;
        public DbSet<Tutor> Tutor { get; set; } = null!;
        public DbSet<AgendamentoServico> AgendamentoServico { get; set; } = null!;
        public DbSet<Logradouro> Logradouro { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Servico>()
                .Property(m => m.Valor)
                .HasPrecision(10, 2);

            modelBuilder.Entity<AgendamentoServico>()
                .HasKey(pm => new { pm.AgendamentoId, pm.ServicoId });

            modelBuilder.Entity<AgendamentoServico>()
                .HasOne(pm => pm.Agendamento)
                .WithMany(p => p.AgendamentoServico)
                .HasForeignKey(pm => pm.AgendamentoId);

            modelBuilder.Entity<AgendamentoServico>()
                .HasOne(pm => pm.Servico)
                .WithMany()
                .HasForeignKey(pm => pm.ServicoId);

            base.OnModelCreating(modelBuilder);
        }
    }
}