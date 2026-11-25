using IngressosService.Models;
using Microsoft.EntityFrameworkCore;

namespace IngressosService
{
    // contexto de banco de dados para o microsserviço de ingressos
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Ingresso> Ingressos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // mapeamento da entidade ingresso
            modelBuilder.Entity<Ingresso>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.CodigoTicket).IsRequired();
                entity.Property(i => i.Status)
                    .HasConversion<string>()
                    .IsRequired();
                entity.Property(i => i.Preco).HasColumnType("TEXT");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
