using System.ComponentModel.DataAnnotations;
using Api.DTOs.Venta;

namespace Api.DTOs.Cliente
{
    /// <summary>
    /// Data Transfer Object for Cliente entity.
    /// Used for transferring cliente data between layers.
    /// </summary>
    public class ClienteDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;

        [MaxLength(20)]
        public string? Telefono { get; set; }

        [MaxLength(25)]
        public string? RTN { get; set; }

        public ICollection<VentaDTO>? Ventas { get; set; }
    }
}