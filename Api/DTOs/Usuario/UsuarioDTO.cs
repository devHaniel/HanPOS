using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Usuario
{
    /// <summary>
    /// Data Transfer Object for Usuario entity.
    /// Used for transferring usuario data between layers.
    /// </summary>
    public class UsuarioDTO
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
        public string PasswordHash { get; set; } = null!;

        public bool Activo { get; set; }
    }
}