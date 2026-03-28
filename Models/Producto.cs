namespace TFGBACKEN.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; } // Para el link de la foto
        
        // El ID del usuario que sube el producto
        public int UsuarioId { get; set; } 
    }
}