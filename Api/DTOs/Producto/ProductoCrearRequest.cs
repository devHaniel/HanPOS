using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Producto
{
    /// <summary>
    /// DTO para crear un nuevo producto (request).
    /// </summary>
    public class ProductoCrearRequest
    {
        [Required]
        [MaxLength(50)]
        public string Codigo { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;

        public decimal PrecioVenta { get; set; } = 0;

        public decimal PrecioCompra { get; set; } = 0;

        public decimal Stock { get; set; } = 0;

        public decimal StockMinimo { get; set; } = 0;

        public bool Activo { get; set; } = true;

        public int? CategoriaId { get; set; }
    }
}