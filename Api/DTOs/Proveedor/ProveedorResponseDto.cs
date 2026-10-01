using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Proveedor
{
    /// <summary>
    /// DTO para representar un proveedor en listados y vistas detalladas.
    /// NO incluye la colección de Compras (se obtiene vía endpoint separado con paginación).
    /// </summary>
    public class ProveedorResponseDto
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