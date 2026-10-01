using System.ComponentModel.DataAnnotations;
using Api.DTOs.Compra;

namespace Api.DTOs.Proveedor
{
    /// <summary>
    /// Data Transfer Object for Proveedor entity.
    /// Used for transferring proveedor data between layers.
    /// </summary>
    public class ProveedorDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;

        [MaxLength(20)]
        public string? Telefono { get; set; }

        [MaxLength(25)]
        public string? RTN { get; set; }

        public ICollection<CompraDTO>? Compras { get; set; }
    }
}