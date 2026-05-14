using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFGBACKEN.Models
{
    // Definimos los estados para no usar "números mágicos" en el código
    public enum EstadoProducto
    {
        Disponible = 0,
        Reservado = 1,
        Vendido = 2
    }

    [Table("productos")]
    public class Producto
    {
        [Key]
        [Column("id_producto")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("caracteristicas")]
        public string? Caracteristicas { get; set; } 

        [Column("categoria_id")]
        public int CategoriaId { get; set; }

        [Column("precio")]
        public decimal Precio { get; set; }

        [Column("imagen_url")]
        public string? ImagenUrl { get; set; }

        [Column("estado_producto")] // Este campo lo mantenemos si lo usas para 'Nuevo/Usado'
        public string? Estado_producto { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; } 

        [Column("vendido")]
        public int Vendido { get; set; } = (int)EstadoProducto.Disponible; // 0=Disp, 1=Reser, 2=Vend
        [Column("comprador_id")]
public int? CompradorId { get; set; }

        [Column("grupo")] 
        public string? Grupo { get; set; }

        [Column("fecha_publicacion")]
        public DateTime FechaPublicacion { get; set; } = DateTime.Now;
    }
}