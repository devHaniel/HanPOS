using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Cliente
{
    /// <summary>
    /// Data Transfer Object for updating a Cliente entity.
    /// Used when updating an existing cliente via API.
    /// </summary>
    public class ClienteActualizarDTO
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