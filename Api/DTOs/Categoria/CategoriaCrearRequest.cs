using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Categoria
{
    /// <summary>
    /// DTO para crear una nueva categoría (request).
    /// </summary>
    public class CategoriaCrearRequest
    {
        [Required]
        [MaxLength(80)]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; } = true;
    }
}