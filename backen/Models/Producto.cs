using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFGBACKEN.Models
{
    [Table("productos")]
    public class Producto
    {
        [Key]
        [Column("id_producto")]
        public int Id { get; set; }

        [Column("nombre")]
        public required string Nombre { get; set; }

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

        [Column("estado_producto")]
        public string? Estado_producto { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; } 

        [Column("vendido")]
public int Vendido { get; set; } // 0 = Libre, 1 = Reservado/Vendido

[Column("Grupo")] // Asegúrate de que esta línea esté bien
        public string? Grupo { get; set; }
        [Column("fecha_publicacion")]
        public DateTime FechaPublicacion { get; set; } = DateTime.Now;
    }
}