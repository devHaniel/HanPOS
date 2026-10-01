using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.DetalleCompra
{
    /// <summary>
    /// DTO para crear un detalle de compra (request).
    /// PrecioUnitario y Subtotal se calculan automáticamente en el servicio.
    /// </summary>
    public class DetalleCompraCrearRequest
    {
        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Cantidad { get; set; }
    }
}