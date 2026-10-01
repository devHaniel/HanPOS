using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Cliente
{
    /// <summary>
    /// DTO para actualizar un cliente existente (request).
    /// </summary>
    public class ClienteActualizarRequest
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