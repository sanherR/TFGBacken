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
        public DbSet<Valoracion> Valoraciones { get; set; }
        public DbSet<Conversacion> Chats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeo de tablas (ajustado a minúsculas o como esté en tu DB)
            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<Producto>().ToTable("productos");
            modelBuilder.Entity<Categoria>().ToTable("categorias");
            modelBuilder.Entity<Favorito>().ToTable("favoritos");
            modelBuilder.Entity<Pedido>().ToTable("pedidos");
            modelBuilder.Entity<Valoracion>().ToTable("valoraciones");
            modelBuilder.Entity<Conversacion>().ToTable("chats");

            // --- CONFIGURACIÓN CRÍTICA PARA MENSAJES ---
            modelBuilder.Entity<Mensaje>(entity =>
            {
                entity.ToTable("mensajes"); // Nombre de la tabla

                entity.Property(e => e.Id).HasColumnName("id_mensaje");
                
                // Forzamos chat_id para que no busque "ConversacionId"
                entity.Property(e => e.Chat_id).HasColumnName("chat_id"); 
                
                entity.Property(e => e.EmisorId).HasColumnName("emisor_id");
                entity.Property(e => e.Contenido).HasColumnName("contenido");
                entity.Property(e => e.Fecha).HasColumnName("fecha_envio");
                
                // Si la columna 'leido' no existe en tu phpMyAdmin, ignórala aquí.
                // Si existe, asegúrate de mapearla:
                // entity.Property(e => e.Leido).HasColumnName("leido");
            });
        }
    }
}