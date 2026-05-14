using TFGBACKEN.Models;

namespace TFGBACKEN.data.interfaces
{
    public interface IConversacionRepository
    {
        Task<int> ObtenerOCrearConversacion(int vendedorId, int compradorId, int productoId);
        
        Task<List<Mensaje>> GetMensajesByConversacion(int conversacionId);
        
        Task<bool> EnviarMensaje(Mensaje mensaje);

        // --- AÑADE ESTA LÍNEA ---
        Task<List<Conversacion>> GetConversacionesByUser(int userId);
    }
}