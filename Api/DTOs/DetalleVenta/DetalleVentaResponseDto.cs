using System;

namespace Api.DTOs.DetalleVenta
{
    /// <summary>
    /// DTO para representar un detalle de venta. Contiene únicamente los datos propios de la entidad DetalleVenta.
    /// La relación con Producto se consulta mediante endpoint específico si se necesita.
    /// </summary>
    public class DetalleVentaResponseDto
    {
        public int Id { get; set; }

        public int VentaId { get; set; }

        public int ProductoId { get; set; }

        public decimal Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }
    }
}