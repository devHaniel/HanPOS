using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.DetalleCompra
{
    /// <summary>
    /// Data Transfer Object for updating a DetalleCompra entity.
    /// Used when updating an existing detalleCompra via API.
    /// </summary>
    public class DetalleCompraActualizarDTO
    {
        public int Id { get; set; }

        public int CompraId { get; set; }

        public int ProductoId { get; set; }

        public decimal Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }
    }
}