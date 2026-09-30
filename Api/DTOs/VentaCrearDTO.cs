using System;
using System.Collections.Generic;

namespace Api.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new Venta entity.
    /// Used when creating a new venta via API.
    /// </summary>
    public class VentaCrearDTO
    {
        public DateTime Fecha { get; set; } = DateTime.Now;

        public decimal Subtotal { get; set; } = 0;

        public decimal Impuesto { get; set; } = 0;

        public decimal Total { get; set; } = 0;

        public int? MetodoPago { get; set; }

        public int? Estado { get; set; } = 1;

        public int UsuarioId { get; set; }

        public int? ClienteId { get; set; }

        public ICollection<DetalleVentaCrearDTO>? Detalles { get; set; }
    }
}