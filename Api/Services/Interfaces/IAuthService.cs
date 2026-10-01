using Api.DTOs.Auth;
using Api.DTOs.Usuario;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de autenticación.
    /// Define la lógica de negocio para autenticación y autorización.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Autentica un usuario y genera un token JWT.
        /// </summary>
        Task<AuthResponseDTO?> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registra un nuevo usuario y genera un token JWT.
        /// </summary>
        Task<AuthResponseDTO?> RegisterAsync(RegisterDTO dto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cambia la contraseña de un usuario.
        /// </summary>
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDTO dto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene el usuario actual por ID.
        /// </summary>
        Task<UsuarioDTO?> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default);
    }
}