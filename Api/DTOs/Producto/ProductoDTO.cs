using System.ComponentModel.DataAnnotations;
using Api.DTOs.DetalleVenta;
using Api.DTOs.DetalleCompra;

namespace Api.DTOs.Producto
{
    /// <summary>
    /// Data Transfer Object for Producto entity.
    /// Used for transferring producto data between layers.
    /// </summary>
    public class ProductoDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Codigo { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;

        public decimal PrecioVenta { get; set; }

        public decimal PrecioCompra { get; set; }

        public decimal Stock { get; set; }

        public decimal StockMinimo { get; set; }

        public bool Activo { get; set; }

        public int? CategoriaId { get; set; }

        public ICollection<DetalleVentaDTO>? DetallesVenta { get; set; }
        public ICollection<DetalleCompraDTO>? DetallesCompra { get; set; }
    }
}