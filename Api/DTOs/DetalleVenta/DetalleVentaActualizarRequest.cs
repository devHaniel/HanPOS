using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.DetalleVenta
{
    /// <summary>
    /// DTO para actualizar un detalle de venta (request).
    /// PrecioUnitario y Subtotal se recalculan automáticamente.
    /// </summary>
    public class DetalleVentaActualizarRequest
    {
        public int Id { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Cantidad { get; set; }
    }
}