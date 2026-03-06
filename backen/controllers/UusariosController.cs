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

        [HttpGet]
        public async Task<IEnumerable<Usuario>> Get() => 
            await _repo.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<Usuario> Get(int id) => 
            await _repo.GetByIdAsync(id);

        [HttpPost]
        public async Task<Usuario> Post([FromBody] Usuario usuario) => 
            await _repo.AddAsync(usuario);

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