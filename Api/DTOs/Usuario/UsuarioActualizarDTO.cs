using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Usuario
{
    /// <summary>
    /// Data Transfer Object for updating a Usuario entity.
    /// Used when updating an existing usuario via API.
    /// </summary>
    public class UsuarioActualizarDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        // PasswordHash es opcional en actualización (solo si se quiere cambiar)
        [MaxLength(256)]
        public string? PasswordHash { get; set; }

        public bool Activo { get; set; }
    }
}