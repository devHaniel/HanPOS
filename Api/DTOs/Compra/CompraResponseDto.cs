using System;

namespace Api.DTOs.Compra
{
    /// <summary>
    /// DTO para representar una compra. Contiene únicamente los datos propios de la entidad Compra.
    /// Las relaciones (Caja, Usuario, Proveedor, Detalles) se consultan mediante endpoints específicos.
    /// </summary>
    public class CompraResponseDto
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Total { get; set; }

        public int Estado { get; set; }

        public int CajaId { get; set; }

        public int UsuarioId { get; set; }

        public int ProveedorId { get; set; }
    }
}