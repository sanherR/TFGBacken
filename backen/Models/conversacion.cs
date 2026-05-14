using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFGBACKEN.Models
{
    [Table("chats")] 
    public class Conversacion
    {
        [Key]
        [Column("id_chat")]
        public int Id { get; set; }

        [Column("vendedor_id")]
        public int VendedorId { get; set; }

        [Column("comprador_id")]
        public int CompradorId { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedad de navegación para el Producto
        [ForeignKey("ProductoId")]
        public virtual Producto Producto { get; set; }

        // --- SOLUCIÓN AL ERROR ConversacionId ---
        // Le decimos a EF que los mensajes de esta conversación 
        // se encuentran buscando "Chat_id" en la tabla de mensajes.
        [ForeignKey("Chat_id")] 
        public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
    }
}