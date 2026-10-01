using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Api.DTOs.DetalleVenta;

namespace Api.DTOs.Venta
{
    /// <summary>
    /// Data Transfer Object for updating a Venta entity.
    /// Used when updating an existing venta via API.
    /// </summary>
    public class VentaActualizarDTO
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

        public ICollection<DetalleVentaActualizarDTO>? Detalles { get; set; }
    }
}