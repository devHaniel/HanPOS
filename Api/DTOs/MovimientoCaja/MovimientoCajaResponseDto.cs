using System;

namespace Api.DTOs.MovimientoCaja
{
    /// <summary>
    /// DTO para representar un movimiento de caja. Contiene únicamente los datos propios de la entidad MovimientoCaja.
    /// La relación con Caja se consulta mediante endpoint específico si se necesita.
    /// </summary>
    public class MovimientoCajaResponseDto
    {
        public int Id { get; set; }

        public decimal Monto { get; set; }

        public int TipoMovimiento { get; set; }

        public string? Concepto { get; set; }

        public DateTime Fecha { get; set; }

        public int CajaId { get; set; }
    }
}