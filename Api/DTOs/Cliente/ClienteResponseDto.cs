using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Cliente
{
    /// <summary>
    /// DTO para representar un cliente en listados y vistas detalladas.
    /// NO incluye la colección de Ventas (se obtiene vía endpoint separado con paginación).
    /// </summary>
    public class ClienteResponseDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;

        [MaxLength(20)]
        public string? Telefono { get; set; }

        [MaxLength(25)]
        public string? RTN { get; set; }
    }
}