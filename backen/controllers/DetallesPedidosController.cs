using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFGBACKEN.Data;
using TFGBACKEN.Models;

namespace TFGBACKEN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetallePedidosController : ControllerBase
    {
        private readonly TfgDbContext _context;
        public DetallePedidosController(TfgDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetallePedido>>> Get() => await _context.DetallesPedidos.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<DetallePedido>> Post(DetallePedido dp)
        {
            _context.DetallesPedidos.Add(dp);
            await _context.SaveChangesAsync();
            return Ok(dp);
        }

        // Endpoint extra para ver los detalles de un pedido específico
        [HttpGet("pedido/{pedidoId}")]
        public async Task<ActionResult<IEnumerable<DetallePedido>>> GetByPedido(int pedidoId)
        {
            return await _context.DetallesPedidos
                .Where(d => d.PedidoId == pedidoId)
                .ToListAsync();
        }
    }
}