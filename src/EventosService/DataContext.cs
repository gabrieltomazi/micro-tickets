using EventosService.Models;
using Microsoft.EntityFrameworkCore;

namespace EventosService
{
    // contexto de banco de dados para o microsserviço de eventos
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Evento> Eventos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // mapeamento da entidade evento
            modelBuilder.Entity<Evento>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired();
                entity.Property(e => e.Local).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
