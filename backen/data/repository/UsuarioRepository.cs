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

        // Obtener todos los usuarios
        public async Task<List<Usuario>> GetAllAsync() => 
            await _context.Usuarios.ToListAsync();

        // Obtener usuario por ID
        public async Task<Usuario> GetByIdAsync(int id) => 
            await _context.Usuarios.FindAsync(id);

        // REGISTRO: Añadir nuevo usuario
        public async Task<Usuario> AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        // Actualizar datos
        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Borrar usuario
        public async Task DeleteAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        // LOGIN: Validar credenciales
        public async Task<Usuario> AuthenticateAsync(string email, string password)
        {
            // Busca el primer usuario que coincida con el email y la contraseña
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.Contraseña == password);
        }
    }
}