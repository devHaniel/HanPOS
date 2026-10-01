using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Api.DTOs.DetalleCompra;

namespace Api.DTOs.Compra
{
    /// <summary>
    /// Data Transfer Object for Compra entity.
    /// Used for transferring compra data between layers.
    /// </summary>
    public class CompraDTO
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

        public ICollection<DetalleCompraDTO>? Detalles { get; set; }
    }
}