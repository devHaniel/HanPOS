using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Negocio
{
    /// <summary>
    /// DTO para representar el negocio en respuestas.
    /// </summary>
    public class NegocioResponseDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; } = null!;

        [MaxLength(200)]
        public string? RazonSocial { get; set; }

        [MaxLength(20)]
        public string? IdentificacionFiscal { get; set; }

        [MaxLength(300)]
        public string? Direccion { get; set; }

        [MaxLength(30)]
        public string? Telefono { get; set; }

        [MaxLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? MensajePieComprobante { get; set; }

        public string? Logo { get; set; }

        public string? PreferenciasPantalla { get; set; }

        public string? ConfiguracionImpresion { get; set; }

        [Required]
        [MaxLength(3)]
        public string MonedaDefecto { get; set; } = "PEN";

        [Required]
        [MaxLength(5)]
        public string SimboloMoneda { get; set; } = "S/";

        [Required]
        [MaxLength(50)]
        public string ZonaHoraria { get; set; } = "America/Lima";

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaActualizacion { get; set; }
    }
}