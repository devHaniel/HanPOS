using System;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    /// <summary>
    /// Data Transfer Object for DetalleCompra entity.
    /// Used for transferring detalleCompra data between layers.
    /// </summary>
    public class DetalleCompraDTO
    {
        public int Id { get; set; }

        public int CompraId { get; set; }

        public int ProductoId { get; set; }

        public decimal Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }
    }
}