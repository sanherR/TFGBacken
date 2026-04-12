using Microsoft.AspNetCore.Mvc;
using TFGBACKEN.Models;
using TFGBACKEN.Repositories;

namespace TFGBACKEN.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioRepository _repo;

        public UsuariosController(UsuarioRepository repo)
        {
            _repo = repo;
        }

        // Obtener todos los usuarios
        [HttpGet]
        public async Task<IEnumerable<Usuario>> Get() => 
            await _repo.GetAllAsync();

        // Obtener un usuario por ID
        [HttpGet("{id}")]
        public async Task<Usuario> Get(int id) => 
            await _repo.GetByIdAsync(id);

        // REGISTRO DE USUARIO (Ya lo tenías, es el POST principal)
        [HttpPost]
        public async Task<Usuario> Post([FromBody] Usuario usuario) => 
            await _repo.AddAsync(usuario);

        // LOGIN DE USUARIO (Nuevo método)
        // La ruta será: api/usuarios/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest datos)
        {
            // Llamamos al método que añadimos en el Repository
            var user = await _repo.AuthenticateAsync(datos.Email, datos.Password);
            
            if (user == null) 
            {
                // Si no lo encuentra, devuelve un error 401 (No autorizado)
                return Unauthorized(new { mensaje = "Correo o contraseña incorrectos" });
            }
            
            // Si todo está bien, devuelve el usuario y un código 200 (OK)
            return Ok(user);
        }

        // Actualizar usuario
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.Id) return BadRequest();
            await _repo.UpdateAsync(usuario);
            return NoContent();
        }

        // Borrar usuario
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }

    // Clase auxiliar para recibir los datos de inicio de sesión desde MAUI
    public class LoginRequest 
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}