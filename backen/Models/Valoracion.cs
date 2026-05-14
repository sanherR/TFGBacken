using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFGBACKEN.Models
{
    [Table("Valoraciones")]
    public class Valoracion
    {
        [Key]
        [Column("id_valoracion")]
        public int Id { get; set; }

        [Column("pedido_id")]
        public int PedidoId { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("puntuacion")]
        public int Puntuacion { get; set; }

        [Column("destinatario_id")]
        public int DestinatarioId { get; set; }

        [Column("remitente_id")]
        public int RemitenteId { get; set; }

        [Column("comentario")]
        public string? Comentario { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}