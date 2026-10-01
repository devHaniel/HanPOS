using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Usuario
{
    /// <summary>
    /// DTO para representar un usuario en listados y vistas detalladas.
    /// NO incluye PasswordHash, SecurityStamp, ConcurrencyStamp ni otras propiedades sensibles de Identity.
    /// NO incluye colecciones de navegación (Cajas, Ventas, Compras) para evitar respuestas grandes.
    /// </summary>
    public class UsuarioResponseDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        public string UserName { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        public string Email { get; set; } = null!;

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}