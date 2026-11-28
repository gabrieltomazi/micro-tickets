using ClientesService.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientesService
{
    // contexto de banco de dados para o microsserviço de clientes
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // mapeamento da entidade cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Nome).IsRequired();
                entity.Property(c => c.Email).IsRequired();
                entity.Property(c => c.Documento).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
