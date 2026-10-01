using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Api.DTOs.DetalleCompra;

namespace Api.DTOs.Compra
{
    /// <summary>
    /// Data Transfer Object for creating a new Compra entity.
    /// Used when creating a new compra via API.
    /// </summary>
    public class CompraCrearDTO
    {
        public DateTime Fecha { get; set; } = DateTime.Now;

        public decimal Subtotal { get; set; } = 0;

        public decimal Impuesto { get; set; } = 0;

        public decimal Total { get; set; } = 0;

        public int Estado { get; set; } = 1;

        public int UsuarioId { get; set; }

        public int CajaId { get; set; }

        public int ProveedorId { get; set; }

        public ICollection<DetalleCompraDTO>? Detalles { get; set; }
    }
}