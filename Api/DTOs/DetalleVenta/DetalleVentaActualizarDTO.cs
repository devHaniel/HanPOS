using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.DetalleVenta
{
    /// <summary>
    /// Data Transfer Object for updating a DetalleVenta entity.
    /// Used when updating an existing detalleVenta via API.
    /// </summary>
    public class DetalleVentaActualizarDTO
    {
        public int Id { get; set; }

        public int VentaId { get; set; }

        public int ProductoId { get; set; }

        public decimal Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }
    }
}