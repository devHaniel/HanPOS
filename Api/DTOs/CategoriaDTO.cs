using System;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
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

        public bool Activo { get; set; }

        public ICollection<ProductoDTO>? Productos { get; set; }
    }
}