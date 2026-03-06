using Microsoft.EntityFrameworkCore;
using TFGBACKEN.Models;


namespace TFGBACKEN.Data
{
    public class TfgDbContext : DbContext
    {
        public TfgDbContext(DbContextOptions<TfgDbContext> options)
            : base(options)
        {
        }

        // Tablas
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Producto>().ToTable("Productos");

        }
    }
}