using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Api.DTOs.DetalleVenta;

namespace Api.DTOs.Venta
{
    /// <summary>
    /// DTO para crear una nueva venta (request).
    /// Incluye los detalles de la venta.
    /// Subtotal, Impuesto y Total se calculan automáticamente en el servicio.
    /// </summary>
    public class VentaCrearRequest
    {
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public int? MetodoPago { get; set; }

        public int? Estado { get; set; } = 1; // Completada por defecto

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int CajaId { get; set; }

        public int? ClienteId { get; set; }

        [Required]
        [MinLength(1)]
        public ICollection<DetalleVentaCrearRequest> Detalles { get; set; } = new List<DetalleVentaCrearRequest>();
    }
}