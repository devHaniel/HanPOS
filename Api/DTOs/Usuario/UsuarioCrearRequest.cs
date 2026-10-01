using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Usuario
{
    /// <summary>
    /// DTO para crear un nuevo usuario (request).
    /// Usa Password (texto plano) en lugar de PasswordHash - el servicio debe hashearlo.
    /// Incluye Email requerido para ASP.NET Identity.
    /// </summary>
    public class UsuarioCrearRequest
    {
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

        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string Password { get; set; } = null!;

        public bool Activo { get; set; } = true;
    }
}