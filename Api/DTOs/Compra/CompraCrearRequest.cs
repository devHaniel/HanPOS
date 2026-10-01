using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Api.DTOs.DetalleCompra;

namespace Api.DTOs.Compra
{
    /// <summary>
    /// DTO para crear una nueva compra (request).
    /// Incluye los detalles de la compra.
    /// Subtotal, Impuesto y Total se calculan automáticamente en el servicio.
    /// </summary>
    public class CompraCrearRequest
    {
        public DateTime Fecha { get; set; } = DateTime.Now;

        public int Estado { get; set; } = 1; // Completada por defecto

        [Required]
        public int UsuarioId { get; set; }

        public int CajaId { get; set; }

        [Required]
        public int ProveedorId { get; set; }

        [Required]
        [MinLength(1)]
        public ICollection<DetalleCompraCrearRequest> Detalles { get; set; } = new List<DetalleCompraCrearRequest>();
    }
}