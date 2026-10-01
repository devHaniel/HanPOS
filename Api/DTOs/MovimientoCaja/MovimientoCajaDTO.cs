using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.MovimientoCaja
{
    /// <summary>
    /// Data Transfer Object for MovimientoCaja entity.
    /// Used for transferring movimientoCaja data between layers.
    /// </summary>
    public class MovimientoCajaDTO
    {
        public int Id { get; set; }

        public decimal Monto { get; set; }

        public int TipoMovimiento { get; set; }

        [MaxLength(200)]
        public string? Concepto { get; set; }

        public DateTime Fecha { get; set; }

        public int CajaId { get; set; }
    }
}