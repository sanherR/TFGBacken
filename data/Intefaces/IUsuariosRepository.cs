using System.Collections.Generic;
using System.Threading.Tasks;
using TFGBACKEN.Models;

namespace TFGBacken.Data.Interfaces
{
    public interface IUsuarioRepository
    {
        // Devuelve todos los usuarios
        Task<List<Usuario>> GetAllAsync();

        // Devuelve un usuario por su Id
        Task<Usuario?> GetByIdAsync(int id);

        // Devuelve usuarios por email
        Task<Usuario?> GetByEmailAsync(string email);

        // Buscar usuarios por nombre
        Task<List<Usuario>> BuscarPorNombreAsync(string nombre);
    }
}