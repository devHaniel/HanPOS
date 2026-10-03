using System;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.MovimientoCaja
{
    /// <summary>
    /// DTO para crear un movimiento de caja (request).
    /// </summary>
    public class MovimientoCajaCrearRequest
    {
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Monto { get; set; }

        [Required]
        public int TipoMovimiento { get; set; } // 1 = Ingreso, 2 = Egreso

        [MaxLength(200)]
        public string? Concepto { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Required]
        public int CajaId { get; set; }
    }
}