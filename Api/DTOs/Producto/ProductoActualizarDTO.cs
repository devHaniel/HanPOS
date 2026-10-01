using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Producto
{
    /// <summary>
    /// Data Transfer Object for updating a Producto entity.
    /// Used when updating an existing producto via API.
    /// </summary>
    public class ProductoActualizarDTO
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