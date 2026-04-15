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

        [Column("categoria_id")]
        public int CategoriaId { get; set; }

        [Column("precio")]
        public decimal Precio { get; set; }

        [Column("imagen_url")]
        public string? ImagenUrl { get; set; } // Para el link de la foto

        

        [Column("Grupo")]
        public string? Grupo { get; set; } // Para agrupar productos similares

        [Column("vendido")]
        public bool vendido { get; set; } 

        [Column("estado_producto")]
        public string? Estado_producto { get; set; }

        // El ID del usuario que sube el producto
        [Column("usuario_id")]
        public int UsuarioId { get; set; } 

        [Column("marca")]
        public string? Marca { get; set; }

        [Column("fecha_publicacion")]
        public DateTime FechaPublicacion { get; set; }
    }
}   