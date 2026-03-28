using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFGBACKEN.Models
{
    public class Valoracion
    {
        [Key]
        [Column("id_valoracion")]
        public int Id { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("puntuacion")]
        public int Puntuacion { get; set; }

        [Column("comentario")]
        public string? Comentario { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}