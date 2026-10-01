using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de Usuario.
    /// Define el contrato para operaciones sobre la entidad Usuario.
    /// </summary>
    public interface IUsuarioRepository
    {
        // Obtener todos los usuarios activos
        Task<IReadOnlyList<Usuario>> GetActivosAsync(
            CancellationToken cancellationToken = default);

        // Obtener usuario por ID
        Task<Usuario?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        // Obtener por username
        Task<Usuario?> GetByUsernameAsync(
            string username,
            CancellationToken cancellationToken = default);

        // Obtener por email
        Task<Usuario?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        // Verificar si username existe
        Task<bool> UsernameExistsAsync(
            string username,
            CancellationToken cancellationToken = default);

        // Verificar si email existe
        Task<bool> EmailExistsAsync(
            string email,
            CancellationToken cancellationToken = default);

        // Agregar nuevo usuario
        Task<Usuario> AddAsync(
            Usuario usuario,
            CancellationToken cancellationToken = default);

        // Eliminar usuario
        Task RemoveAsync(
            Usuario usuario,
            CancellationToken cancellationToken = default);
    }
}