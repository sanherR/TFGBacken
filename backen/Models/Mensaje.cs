using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFGBACKEN.Models
{
    [Table("Mensajes")]
    public class Mensaje
    {
        [Key]
        [Column("id_mensaje")]
        public int Id { get; set; }

        [Column("emisor_id")]
        public int EmisorId { get; set; }

        [Column("receptor_id")]
        public int ReceptorId { get; set; }

        [Column("mensaje")]
        public string Contenido { get; set; } = string.Empty;

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}