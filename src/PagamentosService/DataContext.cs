using PagamentosService.Models;
using Microsoft.EntityFrameworkCore;

namespace PagamentosService
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Pagamento> Pagamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
