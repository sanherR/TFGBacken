using System.Collections.Generic;
using System.Threading.Tasks;
using TFGBACKEN.Models;

namespace TFGBacken.Data.Interfaces
{
    public interface IUsuarioRepository
    {
        // Devuelve todos los usuarios
        Task<List<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario> AddAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(int id);
        Task<Usuario?> AuthenticateAsync(string email, string contrasena);
    }
}