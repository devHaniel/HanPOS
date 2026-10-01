using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Proveedor
{
    /// <summary>
    /// Data Transfer Object for creating a new Proveedor entity.
    /// Used when creating a new proveedor via API.
    /// </summary>
    public class ProveedorCrearDTO
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