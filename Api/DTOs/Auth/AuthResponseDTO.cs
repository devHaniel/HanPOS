using Api.DTOs.Usuario;

namespace Api.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for authentication response with token.
    /// </summary>
    public class AuthResponseDTO
    {
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
        public UsuarioDTO Usuario { get; set; } = null!;
    }
}