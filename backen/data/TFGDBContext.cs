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

            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<Producto>().ToTable("productos");
            modelBuilder.Entity<Categoria>().ToTable("categorias");
            modelBuilder.Entity<Mensaje>().ToTable("mensajes");
            modelBuilder.Entity<Favorito>().ToTable("favoritos");
            modelBuilder.Entity<Pedido>().ToTable("pedidos");
            modelBuilder.Entity<DetallePedido>().ToTable("detalles_pedido");
            modelBuilder.Entity<Valoracion>().ToTable("valoraciones");
        }
    }
}