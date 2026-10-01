using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Producto
{
    /// <summary>
    /// DTO para actualizar un producto existente (request).
    /// </summary>
    public class ProductoActualizarRequest
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
    }
}