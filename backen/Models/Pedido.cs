using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // Importante para el móvil

namespace TFGBACKEN.Models
{
    [Table("pedidos")]
    public class Pedido
    {
        [Key]
        [Column("id_pedido")]
        public int Id { get; set; }

        // ESTA ES LA COLUMNA QUE FALTA Y CAUSA EL ERROR
        [Column("producto_id")]
        [JsonPropertyName("producto_id")] 
        public int ProductoId { get; set; }

        [Column("comprador_id")]
        [JsonPropertyName("comprador_id")]
        public int CompradorId { get; set; }
            
        [Column("vendedor_id")]
        [JsonPropertyName("vendedor_id")]
        public int VendedorId { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Column("total")]
        [JsonPropertyName("total")]
        public decimal Total { get; set; }

        [Column("estado")]
        [JsonPropertyName("estado")]
        public string Estado { get; set; } = "completado";
    }
}