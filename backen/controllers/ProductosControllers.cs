using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFGBACKEN.Data;
using TFGBACKEN.Models;
using System.Security.Claims;

namespace TFGBACKEN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly TfgDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductosController(TfgDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Productos (Para ver todos los anuncios)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.ToListAsync();
        }

    [Authorize]  // POST: api/Productos (Para subir un producto nuevo)
    [HttpPost]
    public async Task<ActionResult<Producto>> PostProducto(
        [FromForm] string nombre,
        [FromForm] string descripcion,
        [FromForm] decimal precio,
        [FromForm] int usuarioId,
        [FromForm] int categoriaId,
        [FromForm] IFormFile imagen)
    {
        Console.WriteLine("AUTH HEADER: " + Request.Headers["Authorization"]);
        Console.WriteLine("AUTH => [" + Request.Headers.Authorization + "]");
        Console.WriteLine("ENTRÓ EN POST PRODUCTO");
        Console.WriteLine("AUTH HEADER: " + Request.Headers.Authorization);
        Console.WriteLine("IS AUTH: " + User.Identity?.IsAuthenticated);
        // Validaciones
        Console.WriteLine("AUTH HEADER RAW:");
        Console.WriteLine(Request.Headers["Authorization"].ToString()); 
        
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(descripcion))
            return BadRequest("Nombre y descripción son obligatorios.");

        if (precio < 0)
            return BadRequest("El precio debe ser mayor o igual a cero.");

        string imagenUrl = null;

        if (imagen != null && imagen.Length > 0)
        {
            try
            {
                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var carpeta = Path.Combine(webRoot, "images");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                var nombreArchivo = Guid.NewGuid() + Path.GetExtension(imagen.FileName);
                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }

                imagenUrl = $"/images/{nombreArchivo}";
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar la imagen: {ex.Message}");
            }
        }

        var producto = new Producto
        {
            Nombre = nombre,
            Descripcion = descripcion,
            Precio = precio,
            ImagenUrl = imagenUrl,
            UsuarioId = usuarioId,
            CategoriaId = categoriaId,
            Grupo = "Recomendados"
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProductos), new { id = producto.Id }, producto);
    }
    }
}