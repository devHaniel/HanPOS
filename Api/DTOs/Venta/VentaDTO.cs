using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Api.DTOs.DetalleVenta;

namespace Api.DTOs.Venta
{
    /// <summary>
    /// Data Transfer Object for Venta entity.
    /// Used for transferring venta data between layers.
    /// </summary>
    public class VentaDTO
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Total { get; set; }

        public int? MetodoPago { get; set; }

        public int? Estado { get; set; }

        public int UsuarioId { get; set; }

        public int CajaId { get; set; }

        public int? ClienteId { get; set; }

        public ICollection<DetalleVentaDTO>? Detalles { get; set; }
    }
}