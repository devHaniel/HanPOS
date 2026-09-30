using System;

namespace Api.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new MovimientoCaja entity.
    /// Used when creating a new movimientoCaja via API.
    /// </summary>
    public class MovimientoCajaCrearDTO
    {
        public decimal Monto { get; set; } = 0;

        public int TipoMovimiento { get; set; }

        [MaxLength(200)]
        public string? Concepto { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public int CajaId { get; set; }
    }
}