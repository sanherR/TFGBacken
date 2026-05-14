using Microsoft.EntityFrameworkCore;
using TFGBACKEN.Models;
using TFGBACKEN.Data; 
using TFGBACKEN.Data.Interfaces; 

namespace TFGBACKEN.Data.Repositories
{
    public class UsuarioRepository : IUsuariosRepository
    {
        // Cambiado de TFGDBContext a TfgDbContext
        private readonly TfgDbContext _context;

        public UsuarioRepository(TfgDbContext context)
        {
            _context = context;
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario> AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            var existing = await _context.Usuarios.FindAsync(usuario.Id);
            if (existing == null) return;

            existing.Nombre = usuario.Nombre;
            existing.Email = usuario.Email;
            existing.Contrasena = usuario.Contrasena;
            existing.Telefono = usuario.Telefono;
            existing.Direccion = usuario.Direccion;
            existing.PerfilUrl = usuario.PerfilUrl;

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

        public async Task<Usuario?> AuthenticateAsync(string email, string contrasena)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.Contrasena == contrasena);
        }
    }
}