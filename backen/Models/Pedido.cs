using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFGBACKEN.Models
{
    public class Pedido
    {
        [Key]
        [Column("id_pedido")]
        public int Id { get; set; }

        [Column("comprador_id")]
        public int CompradorId { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Column("total")]
        public decimal Total { get; set; }

        [Column("estado")]
        public string Estado { get; set; } = "pendiente";
    }
}