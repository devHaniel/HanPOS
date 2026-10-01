using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Api.DTOs.DetalleCompra;

namespace Api.DTOs.Compra
{
    /// <summary>
    /// DTO para actualizar una compra existente (request).
    /// Incluye los detalles de la compra.
    /// Subtotal, Impuesto y Total se recalculan automáticamente si se modifican los detalles.
    /// </summary>
    public class CompraActualizarRequest
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public int Estado { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        public int CajaId { get; set; }

        [Required]
        public int ProveedorId { get; set; }

        public ICollection<DetalleCompraActualizarRequest>? Detalles { get; set; }
    }
}