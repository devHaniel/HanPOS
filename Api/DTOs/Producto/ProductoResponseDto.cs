using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Producto
{
    /// <summary>
    /// DTO para representar un producto. Contiene únicamente los datos propios de la entidad Producto.
    /// Las relaciones (Categoria, DetallesVenta, DetallesCompra) se consultan mediante endpoints específicos.
    /// </summary>
    public class ProductoResponseDto
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