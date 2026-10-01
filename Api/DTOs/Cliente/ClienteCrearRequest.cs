using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Cliente
{
    /// <summary>
    /// DTO para crear un nuevo cliente (request).
    /// </summary>
    public class ClienteCrearRequest
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