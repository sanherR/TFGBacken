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
        // GET: api/productos
[HttpGet]
public async Task<ActionResult> GetProductos()
{
    var productos = await _repository.GetAllAsync();
    
    if (productos == null) return Ok(new List<object>());

    // FILTRO: Solo quitamos los que están vendidos (estado 2)
    // Los disponibles (0) y los reservados (1) SIGUEN SALIENDO
    var listaLimpia = productos
        .Where(p => p.Vendido != 2) // <--- ESTO significa "que no sea igual a 2"
        .Select(p => new {
            id_producto = p.Id, 
            nombre = p.Nombre ?? "Sin nombre",
            descripcion = p.Descripcion ?? "",
            precio = p.Precio,
            imagen_url = p.ImagenUrl ?? "",
            categoria_id = p.CategoriaId,
            vendido = p.Vendido,
            estado_producto = p.Estado_producto,
            caracteristicas = p.Caracteristicas,
            usuario_id = p.UsuarioId 
        }).ToList();

    return Ok(listaLimpia);
}
// GET: api/productos/{id}
[HttpGet("{id}")]
public async Task<ActionResult> GetProducto(int id)
{
    var p = await _repository.GetByIdAsync(id);

    if (p == null) return NotFound("Producto no encontrado");

    // Devolvemos el objeto plano para que coincida con lo que espera MAUI
    var resultado = new {
        id_producto = p.Id, 
        nombre = p.Nombre ?? "Sin nombre",
        descripcion = p.Descripcion ?? "",
        precio = p.Precio,
        imagen_url = p.ImagenUrl ?? "",
        categoria_id = p.CategoriaId,
        vendido = p.Vendido,
        estado_producto = p.Estado_producto,
        caracteristicas = p.Caracteristicas,
        usuario_id = p.UsuarioId  // CRUCIAL: Sin esto el botón no se pone verde
    };

    return Ok(resultado);
}

        // POST: api/productos
[Authorize]
[HttpPost]
[Consumes("multipart/form-data")] // <-- Esto le dice a Swagger que es un formulario con archivo
public async Task<ActionResult<Producto>> PostProducto([FromForm] ProductoUploadRequest request)
{
    // 1. Validaciones básicas
    if (string.IsNullOrWhiteSpace(request.Nombre))
        return BadRequest("Nombre obligatorio.");

    // 2. Usuario desde token
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdClaim == null) return Unauthorized();
    int usuarioId = int.Parse(userIdClaim);

    // 3. Gestión de la imagen
    string? imagenUrl = null;
    if (request.Imagen != null && request.Imagen.Length > 0)
    {
        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var carpeta = Path.Combine(webRoot, "images");
        if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);

        var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(request.Imagen.FileName);
        var ruta = Path.Combine(carpeta, nombreArchivo);

        using (var stream = new FileStream(ruta, FileMode.Create))
        {
            await request.Imagen.CopyToAsync(stream);
        }
        imagenUrl = $"/images/{nombreArchivo}";
    }

    // 4. Crear el objeto final
    var producto = new Producto
    {
        Nombre = request.Nombre,
        Descripcion = request.Descripcion,
        Precio = request.Precio,
        CategoriaId = request.CategoriaId,
        Caracteristicas = request.Caracteristicas ?? "",
        Estado_producto = request.Estado_producto ?? "",
        ImagenUrl = imagenUrl,
        UsuarioId = usuarioId,
        Grupo = "Recomendados",
        Vendido = 0
    };

    await _repository.AddAsync(producto);
    return Ok(producto);
}

// ESTA CLASE VA AL FINAL DEL ARCHIVO (Fuera de la clase ProductosController)
public class ProductoUploadRequest
{
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int CategoriaId { get; set; }
    public string? Caracteristicas { get; set; }
    public string? Estado_producto { get; set; }
    public IFormFile? Imagen { get; set; }
}

        // GET: api/productos/mis-productos
    
        // GET: api/productos/mis-productos
// GET: api/productos/mis-productos
[Authorize]
[HttpGet("mis-productos")]
public async Task<ActionResult<IEnumerable<object>>> GetMisProductos()
{
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdClaim == null) return Unauthorized();

    int userId = int.Parse(userIdClaim);
    var productos = await _repository.GetByUsuarioIdAsync(userId);

    // SOLUCIÓN TOTAL: Renombramos las claves en minúscula y metemos id_producto
    // Esto hace que la App lea el ID real de la BD y mapee bien el estado 'vendido'
    var resultado = productos.Select(p => new
    {
        id_producto = p.Id, // <-- CORREGIDO: Tu app ahora guardará el ID real (4, 5, 6...)
        nombre = p.Nombre ?? "Sin nombre",
        precio = p.Precio,
        descripcion = p.Descripcion ?? "",
        imagen_url = p.ImagenUrl ?? "",
        vendido = p.Vendido, // <-- CORREGIDO: En minúscula para que el 'if (p.Vendido != 2)' funcione
        categoria_id = p.CategoriaId,
        estado_producto = p.Estado_producto ?? "Nuevo",
        caracteristicas = p.Caracteristicas ?? ""
    }).ToList();

    return Ok(resultado);
}
       // DELETE: api/productos/{id}
[Authorize]
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteProducto(int id)
{
    try
    {
        // 1. Buscamos el producto real en la base de datos por el ID de la URL
        var producto = await _repository.GetByIdAsync(id);

        if (producto == null)
        {
            return NotFound($"No se puede eliminar: Producto con ID {id} no encontrado.");
        }

        // 2. Opcional: Verificación de seguridad si quieres que solo el dueño borre
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim != null)
        {
            int usuarioIdActual = int.Parse(userIdClaim);
            if (producto.UsuarioId != usuarioIdActual)
            {
                return Forbid("No tienes permisos para eliminar este producto.");
            }
        }

        // 3. Eliminamos físicamente el registro a través del repositorio
        await _repository.DeleteAsync(id);

        return NoContent(); // Devuelve un 204 limpio (Éxito sin contenido)
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error interno del servidor al eliminar: {ex.Message}");
    }
}
        // PUT: api/productos/{id}
        // PUT: api/productos/{id}
[HttpPut("{id}")]
[Authorize]
[Consumes("multipart/form-data")]
public async Task<IActionResult> ActualizarProducto(int id, [FromForm] ProductoUploadRequest request)
{
    var producto = await _repository.GetByIdAsync(id);

    if (producto == null)
        return NotFound("Producto no encontrado");

    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdClaim == null) return Unauthorized();

    int usuarioId = int.Parse(userIdClaim);

    if (producto.UsuarioId != usuarioId)
        return Forbid();

    // Actualizamos los datos usando el objeto request
    producto.Nombre = request.Nombre ?? producto.Nombre;
    producto.Descripcion = request.Descripcion ?? producto.Descripcion;
    producto.Precio = request.Precio;
    producto.CategoriaId = request.CategoriaId;
    producto.Caracteristicas = request.Caracteristicas ?? producto.Caracteristicas;
    producto.Estado_producto = request.Estado_producto ?? producto.Estado_producto;

    // Gestión de imagen opcional
    if (request.Imagen != null && request.Imagen.Length > 0)
    {
        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var carpeta = Path.Combine(webRoot, "images");
        if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);

        var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(request.Imagen.FileName);
        var ruta = Path.Combine(carpeta, nombreArchivo);

        using (var stream = new FileStream(ruta, FileMode.Create))
        {
            await request.Imagen.CopyToAsync(stream);
        }

        producto.ImagenUrl = $"/images/{nombreArchivo}";
    }

    await _repository.UpdateAsync(producto); 
    
    return Ok(producto);
}
        [HttpPut("{id}/disponibilidad")]
// PUT: api/productos/{id}/estado
[HttpPut("{id}/estado")]
public async Task<IActionResult> ActualizarEstado(int id, [FromBody] int nuevoEstado)
{
    // Buscamos el producto en el repositorio
    var producto = await _repository.GetByIdAsync(id);

    if (producto == null)
        return NotFound("Producto no encontrado");

    // Validamos que el estado sea 0, 1 o 2 para no meter basura en la BD
    if (nuevoEstado < 0 || nuevoEstado > 2)
        return BadRequest("Estado no válido. Use: 0 (Disponible), 1 (Reservado), 2 (Vendido)");

    // Actualizamos la columna 'vendido'
    producto.Vendido = nuevoEstado;

    await _repository.UpdateAsync(producto);

    return Ok(new { 
        mensaje = "Estado actualizado correctamente", 
        productoId = id, 
        nuevoEstado = producto.Vendido 
    });
}
[HttpPut("{id}/aceptar-reserva")]
public async Task<IActionResult> AceptarReserva(int id, [FromBody] int idComprador)
{
    // 1. Buscamos el producto
    var producto = await _repository.GetByIdAsync(id);

    if (producto == null)
        return NotFound("Producto no encontrado");

    // 2. Aplicamos la lógica de reserva oficial
    producto.Vendido = 1;          // 1 = Reservado
    producto.CompradorId = idComprador; // Guardamos quién se lo queda

    // 3. Guardamos los cambios en la BD
    await _repository.UpdateAsync(producto);

    return Ok(new 
    { 
        mensaje = "Reserva aceptada con éxito", 
        productoId = id, 
        compradorId = idComprador 
    });
}
[HttpPut("{id}/cancelar-reserva")]
public async Task<IActionResult> CancelarReserva(int id)
{
    // 1. Buscamos el producto en el repositorio
    var producto = await _repository.GetByIdAsync(id);

    if (producto == null)
    {
        return NotFound("Producto no encontrado");
    }

    // 2. Aplicamos la lógica para liberar el producto
    producto.Vendido = 0;          // 0 = Disponible (Vuelve a aparecer en la tienda)
    producto.CompradorId = null;    // Quitamos al comprador asociado

    // 3. Guardamos los cambios en la base de datos
    await _repository.UpdateAsync(producto);

    return Ok(new 
    { 
        mensaje = "Reserva cancelada correctamente", 
        productoId = id, 
        estado = "Disponible" 
    });
}
[HttpPut("{id}/confirmar-venta")]
public async Task<IActionResult> ConfirmarVenta(int id)
{
    // 1. Buscamos el producto
    var producto = await _repository.GetByIdAsync(id);

    if (producto == null)
        return NotFound("Producto no encontrado");

    // 2. Estado 2 = Vendido (Finalizado)
    producto.Vendido = 2;

    // 3. Guardamos cambios
    await _repository.UpdateAsync(producto);

    return Ok(new { mensaje = "¡Producto vendido con éxito!" });
}
// --- MÉTODO PARA EDITAR PRODUCTOS ---
// --- MÉTODO PARA EDITAR PRODUCTOS CORREGIDO ---
[HttpPut("{id}")]
public async Task<IActionResult> PutProducto(int id, [FromForm] Producto productoDto)
{
    try
    {
        // 1. Buscamos el registro original usando el ID limpio que viene en la ruta de la URL
        var productoExistente = await _repository.GetByIdAsync(id);

        if (productoExistente == null)
        {
            return NotFound($"Producto con ID {id} no encontrado en la base de datos.");
        }

        // 2. Sincronizamos las propiedades directamente sobre el objeto de la BD
        productoExistente.Nombre = productoDto.Nombre;
        productoExistente.Descripcion = productoDto.Descripcion;
        productoExistente.Precio = productoDto.Precio;
        productoExistente.CategoriaId = productoDto.CategoriaId;
        productoExistente.Estado_producto = productoDto.Estado_producto;
        productoExistente.Caracteristicas = productoDto.Caracteristicas;

        // 3. Procesamiento de archivos adjuntos (Imagen)
        if (Request.Form.Files.Count > 0)
        {
            var archivo = Request.Form.Files[0];
            if (archivo != null && archivo.Length > 0)
            {
                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var carpeta = Path.Combine(webRoot, "images");
                if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);

                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName);
                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                productoExistente.ImagenUrl = $"/images/{nombreArchivo}";
            }
        }

        // 4. Guardamos los cambios definitivos en MySql
        await _repository.UpdateAsync(productoExistente);

        return Ok(productoExistente);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error interno del servidor al editar: {ex.Message}");
    }
}
    
}
}