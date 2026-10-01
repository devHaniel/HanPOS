using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Usuario
{
    /// <summary>
    /// Data Transfer Object for creating a new Usuario entity.
    /// Used when creating a new usuario via API.
    /// </summary>
    public class UsuarioCrearDTO
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        public string PasswordHash { get; set; } = null!;

        public bool Activo { get; set; } = true;
    }
}