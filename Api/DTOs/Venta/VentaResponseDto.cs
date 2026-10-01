using System;

namespace Api.DTOs.Venta
{
    /// <summary>
    /// DTO para representar una venta. Contiene únicamente los datos propios de la entidad Venta.
    /// Las relaciones (Caja, Usuario, Cliente, Detalles) se consultan mediante endpoints específicos.
    /// </summary>
    public class VentaResponseDto
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Total { get; set; }

        public int? MetodoPago { get; set; }

        public int? Estado { get; set; }

        public int CajaId { get; set; }

        public int UsuarioId { get; set; }

        public int? ClienteId { get; set; }
    }
}