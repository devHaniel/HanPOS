using System;

namespace Api.DTOs.DetalleCompra
{
    /// <summary>
    /// DTO para representar un detalle de compra. Contiene únicamente los datos propios de la entidad DetalleCompra.
    /// La relación con Producto se consulta mediante endpoint específico si se necesita.
    /// </summary>
    public class DetalleCompraResponseDto
    {
        public int Id { get; set; }

        public int CompraId { get; set; }

        public int ProductoId { get; set; }

        public decimal Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }
    }
}