using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Proveedor
{
    /// <summary>
    /// Data Transfer Object for updating a Proveedor entity.
    /// Used when updating an existing proveedor via API.
    /// </summary>
    public class ProveedorActualizarDTO
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