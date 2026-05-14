using Microsoft.EntityFrameworkCore;
using TFGBACKEN.Models;
using TFGBacken.Data.Interfaces;

namespace TFGBACKEN.Data.Repositories
{
    public class ProductosRepository : IProductosRepository
    {
        private readonly TfgDbContext _context;

        public ProductosRepository(TfgDbContext context)
        {
            _context = context;
        }

        public async Task<List<Producto>> GetAllAsync()
        {
            return await _context.Productos.ToListAsync();
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            return await _context.Productos.FindAsync(id);
        }

        public async Task<List<Producto>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Productos
                .Where(p => p.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<Producto> AddAsync(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return false;

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task UpdateAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }

        Task IProductosRepository.DeleteAsync(int id)
        {
            return DeleteAsync(id);
        }
    }
}