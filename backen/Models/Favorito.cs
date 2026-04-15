using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFGBACKEN.Models
{
    [Table("Favoritos")]
    public class Favorito
    {
        [Key]
        [Column("id_favorito")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }

        [Column("fecha_agregado")]
        public DateTime FechaAgregado { get; set; } = DateTime.Now;
    }
}