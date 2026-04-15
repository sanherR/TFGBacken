using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFGBACKEN.Models
{
    
    [Table("usuarios")]
    public class Usuario
    {
        [Key] 
        [Column("id_usuario")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }
        [Column("correo_electronico")]   
        public string Email { get; set; }
        
        [Column("contrasena")]
        public string Contrasena { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("direccion")]
        public string? Direccion { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

         [Column("foto_perfil")]
        public string? FotoPerfil { get; set; }
    }
}