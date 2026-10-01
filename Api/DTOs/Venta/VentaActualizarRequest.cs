using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Api.DTOs.DetalleVenta;

namespace Api.DTOs.Venta
{
    /// <summary>
    /// DTO para actualizar una venta existente (request).
    /// Incluye los detalles de la venta.
    /// Subtotal, Impuesto y Total se recalculan automáticamente si se modifican los detalles.
    /// </summary>
    public class VentaActualizarRequest
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public int? MetodoPago { get; set; }

        public int? Estado { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int CajaId { get; set; }

        public int? ClienteId { get; set; }

        public ICollection<DetalleVentaActualizarRequest>? Detalles { get; set; }
    }
}