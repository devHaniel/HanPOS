using System;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.MovimientoCaja
{
    /// <summary>
    /// DTO para actualizar un movimiento de caja (request).
    /// </summary>
    public class MovimientoCajaActualizarRequest
    {
        public int Id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Monto { get; set; }

        [Required]
        public int TipoMovimiento { get; set; }

        [MaxLength(200)]
        public string? Concepto { get; set; }

        public DateTime Fecha { get; set; }

        [Required]
        public int CajaId { get; set; }
    }
}