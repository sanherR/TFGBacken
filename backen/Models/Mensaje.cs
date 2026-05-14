using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFGBACKEN.Models
{
    [Table("mensajes")]
public class Mensaje
{
    [Key]
    [Column("id_mensaje")]
    public int Id { get; set; }

    [Column("chat_id")] // <--- Esto obliga a usar tu columna real de phpMyAdmin
    public int Chat_id { get; set; } 

    [Column("emisor_id")]
    public int EmisorId { get; set; }

    [Column("contenido")]
    public string Contenido { get; set; } 

    [Column("fecha_envio")]
    public DateTime Fecha { get; set; } = DateTime.Now;
}
}

