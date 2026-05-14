using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TFGBACKEN.Models;
using TFGBACKEN.Data.Repositories;
using TFGBACKEN.Services;
using System.Security.Claims;

namespace TFGBACKEN.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly UsuarioRepository _repo;

        public UsuariosController(UsuarioRepository repo, JwtService jwtService)
        {
            _repo = repo;
            _jwtService = jwtService;
        }

        // 1. Obtener todos los usuarios
        [HttpGet]
        public async Task<IEnumerable<Usuario>> Get() => await _repo.GetAllAsync();

        // 2. Obtener un usuario por ID
        [HttpGet("{id}")]
        public async Task<Usuario?> Get(int id) => await _repo.GetByIdAsync(id);

        // 3. Registro de Usuario (POST principal)
        [HttpPost]
        public async Task<Usuario> Post([FromBody] Usuario usuario) => await _repo.AddAsync(usuario);

        // 4. LOGIN con generación de TOKEN JWT
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest datos)
        {
            // Nota: usamos 'Contrasena' (sin ñ) para coincidir con el repositorio
            var user = await _repo.AuthenticateAsync(datos.Email, datos.Password);
            
            if (user == null) 
                return Unauthorized(new { mensaje = "Correo o contraseña incorrectos" });
            
            var token = _jwtService.GenerateToken(user);
            
            return Ok(new LoginResponse
            {
                Token = token,
                UsuarioId = user.Id,
                Nombre = user.Nombre,
                Email = user.Email
            });
        }

        // 5. Actualizar usuario
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.Id) return BadRequest();
            await _repo.UpdateAsync(usuario);
            return NoContent();
        }

        // 6. Borrar usuario
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteAsync(id);
            return NoContent();
        }

        // 7. Subida de imagen de perfil (Requiere estar logueado)
        [Authorize]
        [HttpPost("upload-profile-image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile imagen)
        {
            if (imagen == null || imagen.Length == 0)
                return BadRequest("Imagen inválida");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profiles");

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imagen.FileName)}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            var url = $"{Request.Scheme}://{Request.Host}/profiles/{fileName}";
            var user = await _repo.GetByIdAsync(userId);

            if (user == null) return NotFound();

            user.PerfilUrl = url;
            await _repo.UpdateAsync(user);

            return Ok(url);
        }

        // 8. Obtener perfil del usuario actual (basado en el token)
        [Authorize]
        [HttpGet("perfil")]
        public async Task<IActionResult> GetPerfil()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var user = await _repo.GetByIdAsync(userId);

            if (user == null) return NotFound();

            return Ok(new { user.Id, user.Nombre, user.Email, user.PerfilUrl });
        }
    }

    // Clases de apoyo para la API
    public class LoginRequest 
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
    }
}