using TFGBACKEN.Models;

namespace TFGBacken.Data.Interfaces
{
    public interface IFavoritosRepository
    {
        Task<bool> ExisteAsync(int usuarioId, int productoId);
        Task AddAsync(Favorito favorito);
        Task<Favorito?> GetAsync(int usuarioId, int productoId);
        Task DeleteAsync(Favorito favorito);
        Task<List<Producto>> GetProductosFavoritosAsync(int usuarioId);
    }
}