using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFGBACKEN.Data;
using TFGBACKEN.Models;
using System.Security.Claims;

namespace TFGBACKEN.Controllers
{
    // Clase DTO para recibir los datos del formulario (incluyendo la imagen)
    // Esto soluciona el error de generación de Swagger
    public class ProductoUploadDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int UsuarioId { get; set; }
        public int CategoriaId { get; set; }
        public IFormFile Imagen { get; set; }
    }

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

        // GET: api/Productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.ToListAsync();
        }

        [Authorize]
[HttpPost]
public async Task<ActionResult<Producto>> PostProducto([FromForm] ProductoUploadDto dto)
{
    // 1. Validaciones básicas
    if (string.IsNullOrWhiteSpace(dto.Nombre))
    {
        return BadRequest("El nombre es obligatorio.");
    }

    string? imagenUrl = null;

    // 2. Lógica de imagen (Simplificada para que veas el error)
    if (dto.Imagen != null && dto.Imagen.Length > 0)
    {
        var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(dto.Imagen.FileName);
        var ruta = Path.Combine(_env.WebRootPath ?? "wwwroot", "images", nombreArchivo);
        using (var stream = new FileStream(ruta, FileMode.Create))
        {
            await dto.Imagen.CopyToAsync(stream);
        }
        imagenUrl = $"/images/{nombreArchivo}";
    }

    // 3. AQUÍ ESTÁ EL CAMBIO: Llamamos a la variable 'nuevoProducto' 
    // para que no se confunda con la clase 'Producto'
    var nuevoProducto = new Producto
    {
        Nombre = dto.Nombre,
        Descripcion = dto.Descripcion,
        Precio = dto.Precio,
        ImagenUrl = imagenUrl,
        UsuarioId = dto.UsuarioId, 
        CategoriaId = dto.CategoriaId,
        Grupo = "Recomendados",
        FechaPublicacion = DateTime.Now 
    };

    // 4. Guardar
    _context.Productos.Add(nuevoProducto);
    await _context.SaveChangesAsync();

    // 5. Retornar (Usamos el ID de la variable nueva)
    return CreatedAtAction(nameof(GetProductos), new { id = nuevoProducto.Id }, nuevoProducto);
}
    }
}