using System;

namespace Api.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new Producto entity.
    /// Used when creating a new producto via API.
    /// </summary>
    public class ProductoCrearDTO
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