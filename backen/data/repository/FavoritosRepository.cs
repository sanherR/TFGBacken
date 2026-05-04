using Microsoft.EntityFrameworkCore;
using TFGBACKEN.Data;
using TFGBACKEN.Models;
using TFGBacken.Data.Interfaces;

namespace TFGBACKEN.Data.Repositories
{
    public class FavoritosRepository : IFavoritosRepository
    {
        private readonly TfgDbContext _context;

        public FavoritosRepository(TfgDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteAsync(int usuarioId, int productoId)
        {
            return await _context.Favoritos
                .AnyAsync(f => f.UsuarioId == usuarioId && f.ProductoId == productoId);
        }

        public async Task AddAsync(Favorito favorito)
        {
            _context.Favoritos.Add(favorito);
            await _context.SaveChangesAsync();
        }

        public async Task<Favorito?> GetAsync(int usuarioId, int productoId)
        {
            return await _context.Favoritos
                .FirstOrDefaultAsync(f => f.UsuarioId == usuarioId && f.ProductoId == productoId);
        }

        public async Task DeleteAsync(Favorito favorito)
        {
            _context.Favoritos.Remove(favorito);
            await _context.SaveChangesAsync();
        }

       public async Task<List<Producto>> GetProductosFavoritosAsync(int usuarioId)
{
            return await _context.Favoritos
                .Where(f => f.UsuarioId == usuarioId)
                .Include(f => f.Producto)
                .Where(f => f.Producto != null)
                .Select(f => f.Producto)
                .ToListAsync();
        }
    }
}   