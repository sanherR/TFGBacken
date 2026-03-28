using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFGBACKEN.Data;
using TFGBACKEN.Models;

namespace TFGBACKEN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensajesController : ControllerBase
    {
        private readonly TfgDbContext _context;
        public MensajesController(TfgDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mensaje>>> Get() => await _context.Mensajes.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Mensaje>> Post(Mensaje m)
        {
            _context.Mensajes.Add(m);
            await _context.SaveChangesAsync();
            return Ok(m);
        }
    }
}