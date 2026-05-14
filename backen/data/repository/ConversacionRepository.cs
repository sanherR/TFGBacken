using Microsoft.EntityFrameworkCore;
using TFGBACKEN.data.interfaces;
using TFGBACKEN.Models;
using TFGBACKEN.Data;

namespace TFGBACKEN.data.repository
{
    public class ConversacionRepository : IConversacionRepository
    {
        private readonly TfgDbContext _context;

        public ConversacionRepository(TfgDbContext context)
        {
            _context = context;
        }

        // 1. Lógica para la Bandeja de Entrada
        public async Task<List<Conversacion>> GetConversacionesByUser(int userId)
        {
            // Usamos .Chats porque así lo nombramos en el DbContext
            return await _context.Chats
                .Include(c => c.Producto) 
                .Include(c => c.Mensajes) 
                .Where(c => c.VendedorId == userId || c.CompradorId == userId)
                .AsNoTracking() 
                .ToListAsync();
        }

        // 2. Obtener o Crear conversación
        public async Task<int> ObtenerOCrearConversacion(int vendedorId, int compradorId, int productoId)
        {
            // Cambiado a _context.Chats
            var chatExistente = await _context.Chats
                .FirstOrDefaultAsync(c => c.VendedorId == vendedorId && 
                                          c.CompradorId == compradorId && 
                                          c.ProductoId == productoId);

            if (chatExistente != null) return chatExistente.Id;

            var nuevoChat = new Conversacion
            {
                VendedorId = vendedorId,
                CompradorId = compradorId,
                ProductoId = productoId
            };

            _context.Chats.Add(nuevoChat);
            await _context.SaveChangesAsync();

            return nuevoChat.Id;
        }

        // 3. Obtener mensajes
        public async Task<List<Mensaje>> GetMensajesByConversacion(int conversacionId)
        {
            return await _context.Mensajes
                // Cambiado m.Conversacion por m.Chat_id
                .Where(m => m.Chat_id == conversacionId)
                .OrderBy(m => m.Fecha)
                .ToListAsync();
        }

        // 4. Enviar mensaje
        public async Task<bool> EnviarMensaje(Mensaje mensaje)
        {
            _context.Mensajes.Add(mensaje);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}