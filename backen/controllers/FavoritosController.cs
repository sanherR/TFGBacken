using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFGBACKEN.Data;
using TFGBACKEN.Models;

namespace TFGBACKEN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritosController : ControllerBase
    {
        private readonly TfgDbContext _context;
        public FavoritosController(TfgDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Favorito>>> Get() => await _context.Favoritos.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Favorito>> Post(Favorito f)
        {
            _context.Favoritos.Add(f);
            await _context.SaveChangesAsync();
            return Ok(f);
        }
    }
}