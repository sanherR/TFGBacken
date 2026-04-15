namespace TFGBACKEN.Models
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
    }
}