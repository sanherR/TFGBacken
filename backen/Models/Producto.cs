using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFGBACKEN.Models
{
    public class Producto
    {
        [Key]
        [Column("id_producto")]
        public int Id { get; set; }
      
        [Column("nombre")]
        public string Nombre { get; set; }
        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("categoria_id")]
        public int Categoria { get; set; } // Para categorizar los productos
        
        [Column("precio")]
        public decimal Precio { get; set; }
        
        [Column("marca")]
        public string? Marca { get; set; } // Para la marca del producto

        [Column("stock")]
        public int stock { get; set; } // Para controlar el stock del producto
        [Column("imagen_url")]
        public string? ImagenUrl { get; set; } // Para el link de la foto

        [Column("grupo")]
        public string? Grupo { get; set; } // Para agrupar los productos

        // El ID del usuario que sube el producto
        [Column("usuario_id")]
        public int UsuarioId { get; set; } 
    }
}   