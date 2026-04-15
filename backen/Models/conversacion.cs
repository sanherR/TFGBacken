using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFGBACKEN.Models
{
    public class Conversacion
    {
        [Key]
        [Column("id_conversacion")]
        public int Id { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }

        [Column("comprador_id")]
        public int Usuario1Id { get; set; }

        [Column("vendedor_id")]
        public int Usuario2Id { get; set; }
        
        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}