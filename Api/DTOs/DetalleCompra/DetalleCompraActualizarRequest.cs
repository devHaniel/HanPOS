using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.DetalleCompra
{
    /// <summary>
    /// DTO para actualizar un detalle de compra (request).
    /// PrecioUnitario y Subtotal se recalculan automáticamente.
    /// </summary>
    public class DetalleCompraActualizarRequest
    {
        public int Id { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Cantidad { get; set; }
    }
}