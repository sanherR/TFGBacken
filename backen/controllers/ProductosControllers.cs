using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TFGBACKEN.Models;
using TFGBacken.Data.Interfaces;

namespace TFGBACKEN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductosRepository _repository;
        private readonly IWebHostEnvironment _env;

        public ProductosController(IProductosRepository repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            var productos = await _repository.GetAllAsync();
            return Ok(productos);
        }

        // POST: api/productos
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(
            [FromForm] string nombre,
            [FromForm] string descripcion,
            [FromForm] decimal precio,
            [FromForm] int categoriaId,
            [FromForm] string caracteristicas, // <--- AÑADIDO
            [FromForm] string estado_producto, // <--- AÑADIDO
            [FromForm] IFormFile imagen)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(descripcion))
                return BadRequest("Nombre y descripción son obligatorios.");

            if (precio < 0)
                return BadRequest("El precio no puede ser negativo.");

            // 🔐 Usuario desde token (NO desde frontend)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int usuarioId = int.Parse(userIdClaim);

            // 📷 Guardar imagen
            string imagenUrl = null;

            if (imagen != null && imagen.Length > 0)
            {
                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var carpeta = Path.Combine(webRoot, "images");

                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                var nombreArchivo = Guid.NewGuid() + Path.GetExtension(imagen.FileName);
                var ruta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }

                imagenUrl = $"/images/{nombreArchivo}";
            }

            var producto = new Producto
            {
                Nombre = nombre,
                Descripcion = descripcion,
                Caracteristicas = caracteristicas, // <--- AÑADIDO
                Estado_producto = estado_producto, // <--- AÑADIDO
                Precio = precio,
                ImagenUrl = imagenUrl,
                UsuarioId = usuarioId,
                CategoriaId = categoriaId,
                Grupo = "Recomendados"
            };

            await _repository.AddAsync(producto);

            return Ok(producto);
        }

        // GET: api/productos/mis-productos
        [Authorize]
        [HttpGet("mis-productos")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetMisProductos()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var productos = await _repository.GetByUsuarioIdAsync(userId);

            return Ok(productos);
        }

        // DELETE: api/productos/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _repository.GetByIdAsync(id);

            if (producto == null)
                return NotFound("Producto no encontrado");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int usuarioId = int.Parse(userIdClaim);

            if (producto.UsuarioId != usuarioId)
                return Forbid();

            await _repository.DeleteAsync(id);

            return Ok("Producto eliminado");
        }

        // PUT: api/productos/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> ActualizarProducto( // <--- Le cambié el nombre a ActualizarProducto
            int id, // <--- Faltaba pedir el ID aquí para que no diera error rojo abajo
            [FromForm] string nombre,
            [FromForm] string descripcion,
            [FromForm] decimal precio,
            [FromForm] int categoriaId,
            [FromForm] string caracteristicas, // <--- AÑADIDO
            [FromForm] string estado_producto, // <--- AÑADIDO
            [FromForm] IFormFile imagen)
        {
            var producto = await _repository.GetByIdAsync(id);

            if (producto == null)
                return NotFound("Producto no encontrado");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int usuarioId = int.Parse(userIdClaim);

            if (producto.UsuarioId != usuarioId)
                return Forbid();

            // Actualizamos los datos del producto existente
            producto.Nombre = nombre;
            producto.Descripcion = descripcion;
            producto.Caracteristicas = caracteristicas; // <--- AÑADIDO
            producto.Estado_producto = estado_producto; // <--- AÑADIDO
            producto.Precio = precio;
            producto.CategoriaId = categoriaId;
            // Quitamos el Grupo = "Recomendados" porque al editar no hace falta sobreescribirlo

            // imagen opcional
            if (imagen != null && imagen.Length > 0)
            {
                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var carpeta = Path.Combine(webRoot, "images");

                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                var nombreArchivo = Guid.NewGuid() + Path.GetExtension(imagen.FileName);
                var ruta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }

                producto.ImagenUrl = $"/images/{nombreArchivo}";
            }

            // Usamos UpdateAsync en vez de AddAsync porque estamos editando
            await _repository.UpdateAsync(producto); 
            
            return Ok(producto);
        }
    }
}