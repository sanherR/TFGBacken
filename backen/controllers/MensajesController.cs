using Microsoft.AspNetCore.Mvc;
using TFGBACKEN.data.interfaces;
using TFGBACKEN.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json.Serialization;

namespace TFGBACKEN.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensajesController : ControllerBase
    {
        private readonly IConversacionRepository _repository;

        public MensajesController(IConversacionRepository repository)
        {
            _repository = repository;
        }

        // 1. Obtener o crear conversación (Al pulsar "Contactar")
        [HttpPost("get-or-create")]
        public async Task<IActionResult> GetOrCreateChat([FromBody] ChatRequest request)
        {
            var id = await _repository.ObtenerOCrearConversacion(
                request.vendedor_id, 
                request.comprador_id, 
                request.producto_id);
                
            return Ok(new { id_chat = id });
        }

        // 2. Obtener mensajes de un chat (Al abrir la ventana de chat)
        [HttpGet("{chatId}")]
        public async Task<ActionResult<List<Mensaje>>> GetMensajes(int chatId)
        {
            var mensajes = await _repository.GetMensajesByConversacion(chatId);
            return Ok(mensajes);
        }

        // 3. Guardar un mensaje nuevo (Al dar a "Enviar")
        [HttpPost]
        public async Task<IActionResult> PostMensaje([FromBody] Mensaje mensaje)
        {
            var exito = await _repository.EnviarMensaje(mensaje);
            if (exito) return Ok();
            return BadRequest("No se pudo guardar el mensaje");
        }

        // 4. Obtener la bandeja de entrada (Para la pestaña "Mensajes")
        [HttpGet("mis-chats/{userId}")]
        public async Task<IActionResult> GetMisChats(int userId)
        {
            var chats = await _repository.GetConversacionesByUser(userId);
            
            if (chats == null) return Ok(new List<object>());

            // Mapeo manual para evitar ciclos infinitos y errores de nulos
            var resultado = chats.Select(c => new 
            {
                Id = c.Id,
                // El "?" asegura que si el producto reservado no carga, no explote
                TituloChat = c.Producto?.Nombre ?? "Producto no disponible",
                ImagenProductoUrl = c.Producto?.ImagenUrl ?? "default_product.png",
                
                // Obtenemos el último mensaje de forma segura
                UltimoMensaje = (c.Mensajes != null && c.Mensajes.Any()) 
                                ? c.Mensajes.OrderByDescending(m => m.Fecha).FirstOrDefault()?.Contenido 
                                : "Sin mensajes aún",
                
                ProductoId = c.ProductoId
            }).ToList(); // Importante: convertir a lista antes de enviar

            return Ok(resultado);
        }
    }

    public class ChatRequest
{
    [JsonPropertyName("vendedor_id")]
    public int vendedor_id { get; set; }

    [JsonPropertyName("comprador_id")]
    public int comprador_id { get; set; }

    [JsonPropertyName("producto_id")]
    public int producto_id { get; set; }
}
}