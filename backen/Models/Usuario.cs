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

        [Column("nombre_usuario")]
        public string nombre_usuario { get; set; } // Para el nombre de usuario único

        [Column("direccion")]
        public string direccion { get; set; } // Para la dirección del usuario

        [Column("telefono")]
        public string? Telefono { get; set; } // Para el número de teléfono del usuario

        [Column("contrasena")]
        public string Contraseña { get; set; }
    }
}