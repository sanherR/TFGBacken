using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFGBACKEN.Models
{
    [Table("Mensajes")]
    public class Mensaje
    {
        [Key]
        [Column("mensaje_id")]
        public int Id { get; set; }
        
        [Column("conversacion_id")]
        public int Conversacion { get; set; } 

        [Column("emisor_id")]
        public int EmisorId { get; set; }

        [Column("mensaje")]
        public string Contenido { get; set; } 

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Column("leido")]
        public bool Leido { get; set; } 
    }
}