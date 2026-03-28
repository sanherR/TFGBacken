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

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Mensaje> Mensajes { get; set; }
        public DbSet<Favorito> Favoritos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedidos { get; set; }
        public DbSet<Valoracion> Valoraciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Producto>().ToTable("Productos");
            modelBuilder.Entity<Categoria>().ToTable("Categorias");
            modelBuilder.Entity<Mensaje>().ToTable("Mensajes");
            modelBuilder.Entity<Favorito>().ToTable("Favoritos");
            modelBuilder.Entity<Pedido>().ToTable("Pedidos");
            modelBuilder.Entity<DetallePedido>().ToTable("Detalles_Pedido");
            modelBuilder.Entity<Valoracion>().ToTable("Valoraciones");
        }
    }
}