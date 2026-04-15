using TFGBACKEN.Data;
using TFGBACKEN.Models;
using Microsoft.EntityFrameworkCore;

namespace TFGBACKEN.Repositories
{
    public class UsuarioRepository
    {
        private readonly TfgDbContext _context;

        public UsuarioRepository(TfgDbContext context)
        {
            _context = context;
        }

        public async Task<List<Usuario>> GetAllAsync() => 
            await _context.Usuarios.ToListAsync();

        public async Task<Usuario> GetByIdAsync(int id) => 
            await _context.Usuarios.FindAsync(id);

        public async Task<Usuario> AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }
         public async Task<Usuario> AuthenticateAsync(string email, string Contrasena)
        {
            // Busca el primer usuario que coincida con el email y la contraseña
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.Contrasena == Contrasena);
        }
    }
}