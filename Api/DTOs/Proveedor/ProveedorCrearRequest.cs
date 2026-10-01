using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Proveedor
{
    /// <summary>
    /// DTO para crear un nuevo proveedor (request).
    /// </summary>
    public class ProveedorCrearRequest
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;

        [MaxLength(20)]
        public string? Telefono { get; set; }

        [MaxLength(25)]
        public string? RTN { get; set; }
    }
}