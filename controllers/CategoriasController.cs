using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFGBACKEN.Data;
using TFGBACKEN.Models;

namespace TFGBACKEN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly TfgDbContext _context;
        public CategoriasController(TfgDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> Get() => await _context.Categorias.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Categoria>> Post(Categoria c)
        {
            _context.Categorias.Add(c);
            await _context.SaveChangesAsync();
            return Ok(c);
        }
    }
}