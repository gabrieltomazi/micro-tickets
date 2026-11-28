using Microsoft.EntityFrameworkCore;
using PagamentosService.Models;

namespace PagamentosService
{
    // contexto de banco de dados para o microsserviço de pagamentos
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Pagamento> Pagamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // mapeamento da entidade pagamento
            modelBuilder.Entity<Pagamento>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.IngressoId).IsRequired();
                entity.Property(p => p.MetodoPagamento).IsRequired();
                entity.Property(p => p.Status).IsRequired();
                entity.Property(p => p.Valor).HasColumnType("TEXT");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
