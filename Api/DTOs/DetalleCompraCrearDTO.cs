using System;

namespace Api.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new DetalleCompra entity.
    /// Used when creating a new detalleCompra via API.
    /// </summary>
    public class DetalleCompraCrearDTO
    {
        public int ProductoId { get; set; }

        public decimal Cantidad { get; set; } = 0;

        public decimal PrecioUnitario { get; set; } = 0;

        public decimal Subtotal { get; set; } = 0;
    }
}