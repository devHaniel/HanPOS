using System;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    /// <summary>
    /// Data Transfer Object for DetalleVenta entity.
    /// Used for transferring detalleVenta data between layers.
    /// </summary>
    public class DetalleVentaDTO
    {
        public int Id { get; set; }

        public int VentaId { get; set; }

        public int ProductoId { get; set; }

        public decimal Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }
    }
}