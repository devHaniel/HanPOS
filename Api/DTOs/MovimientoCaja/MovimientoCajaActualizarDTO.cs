using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.MovimientoCaja
{
    /// <summary>
    /// Data Transfer Object for updating a MovimientoCaja entity.
    /// Used when updating an existing movimientoCaja via API.
    /// </summary>
    public class MovimientoCajaActualizarDTO
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