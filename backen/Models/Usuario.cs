using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFGBACKEN.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key] 
        [Column("id_usuario")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("correo_electronico")]
        public string Email { get; set; }

        // Añadimos '?' porque un usuario podría no tener estos datos al principio
        [Column("nombre_usuario")]
        public string? nombre_usuario { get; set; } 

        [Column("direccion")]
        public string? direccion { get; set; } 

        [Column("telefono")]
        public string? Telefono { get; set; } 

        [Column("contrasena")]
        public string Contraseña { get; set; }
    }
}