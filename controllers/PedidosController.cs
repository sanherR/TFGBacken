using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFGBACKEN.Data;
using TFGBACKEN.Models;

namespace TFGBACKEN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly TfgDbContext _context;
        public PedidosController(TfgDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> Get() => await _context.Pedidos.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Pedido>> Post(Pedido p)
        {
            _context.Pedidos.Add(p);
            await _context.SaveChangesAsync();
            return Ok(p);
        }
    }
}