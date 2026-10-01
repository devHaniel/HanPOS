using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Api.DTOs.DetalleCompra;

namespace Api.DTOs.Compra
{
    /// <summary>
    /// Data Transfer Object for updating a Compra entity.
    /// Used when updating an existing compra via API.
    /// </summary>
    public class CompraActualizarDTO
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Total { get; set; }

        public int Estado { get; set; }

        public int UsuarioId { get; set; }

        public int CajaId { get; set; }

        public int ProveedorId { get; set; }

        public ICollection<DetalleCompraActualizarDTO>? Detalles { get; set; }
    }
}