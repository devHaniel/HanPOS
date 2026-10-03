using System.ComponentModel.DataAnnotations;
using Api.DTOs.Producto;

namespace Api.DTOs.Categoria
{
    /// <summary>
    /// Data Transfer Object for Categoria entity.
    /// Used for transferring categoria data between layers.
    /// </summary>
    public class CategoriaDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(80)]
        public string Nombre { get; set; } = null!;

        [MaxLength(250)]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; }
    }
}