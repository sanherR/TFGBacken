using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFGBACKEN.Data;
using TFGBACKEN.Models;

namespace TFGBACKEN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValoracionesController : ControllerBase
    {
        private readonly TfgDbContext _context;
        public ValoracionesController(TfgDbContext context) { _context = context; }

        [HttpPost]
        public async Task<ActionResult<Valoracion>> Post(Valoracion v)
        {
            v.Fecha = DateTime.Now;
            _context.Valoraciones.Add(v);
            await _context.SaveChangesAsync();
            return Ok(v);
        }
    }
}