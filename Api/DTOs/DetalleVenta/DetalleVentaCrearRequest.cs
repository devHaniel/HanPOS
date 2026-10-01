using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.DetalleVenta
{
    /// <summary>
    /// DTO para crear un detalle de venta (request).
    /// PrecioUnitario y Subtotal se calculan automáticamente en el servicio.
    /// </summary>
    public class DetalleVentaCrearRequest
    {
        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Cantidad { get; set; }
    }
}