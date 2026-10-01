using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Usuario
{
    /// <summary>
    /// DTO para actualizar un usuario existente (request).
    /// Password es opcional - solo se proporciona si se quiere cambiar.
    /// NO incluye PasswordHash.
    /// </summary>
    public class UsuarioActualizarRequest
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        // Opcional - solo si se desea cambiar la contraseña
        [MinLength(6)]
        [MaxLength(100)]
        public string? Password { get; set; }

        public bool Activo { get; set; }
    }
}