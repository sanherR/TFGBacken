using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TFGBACKEN.Models;
using TFGBACKEN.Repositories;
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

        [HttpGet]
        public async Task<IEnumerable<Usuario>> Get() => 
            await _repo.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<Usuario> Get(int id) => 
            await _repo.GetByIdAsync(id);

        [HttpPost]
        public async Task<Usuario> Post([FromBody] Usuario usuario) => 
        
            await _repo.AddAsync(usuario);

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest datos)
        {
            // Llamamos al método que añadimos en el Repository
            var user = await _repo.AuthenticateAsync(datos.Email, datos.Contrasena);
            
            if (user == null) 
            return Unauthorized();
            var token = _jwtService.GenerateToken(user);
            return Ok(new LoginResponse
            {
                Token = token,
                UsuarioId = user.Id,
                Nombre = user.Nombre,
                Email = user.Email
            });
           
            

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.Id) return BadRequest();
            await _repo.UpdateAsync(usuario);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}