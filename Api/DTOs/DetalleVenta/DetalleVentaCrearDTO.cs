using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.DetalleVenta
{
    /// <summary>
    /// Data Transfer Object for creating a new DetalleVenta entity.
    /// Used when creating a new detalleVenta via API.
    /// </summary>
    public class DetalleVentaCrearDTO
    {
        public int ProductoId { get; set; }

        public decimal Cantidad { get; set; } = 0;

        public decimal PrecioUnitario { get; set; } = 0;

        public decimal Subtotal { get; set; } = 0;
    }
}