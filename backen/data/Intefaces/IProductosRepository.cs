using TFGBACKEN.Models;
namespace TFGBacken.Data.Interfaces
{
    public interface IProducotosRepository
    {
        Task<List<Producto>> GetAllAsync();
        Task<Producto?> GetByIdAsync(int id);
        Task<Producto> AddAsync(Producto producto);
        Task DeleteAsync(int id);
    
    }

}