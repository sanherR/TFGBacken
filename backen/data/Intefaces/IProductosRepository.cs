using TFGBACKEN.Models;
namespace TFGBacken.Data.Interfaces
{
    public interface IProductosRepository
    {
        Task<List<Producto>> GetAllAsync();
        Task<Producto?> GetByIdAsync(int id);
        Task<Producto> AddAsync(Producto producto);
        Task DeleteAsync(int id);
        Task<List<Producto>> GetByUsuarioIdAsync(int usuarioId);
        Task UpdateAsync(Producto producto);
    }

}