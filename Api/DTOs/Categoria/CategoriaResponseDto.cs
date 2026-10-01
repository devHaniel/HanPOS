using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Categoria
{
    /// <summary>
    /// DTO para representar una categoría en listados y vistas detalladas.
    /// NO incluye la colección de Productos (se obtiene vía endpoint separado con paginación).
    /// </summary>
    public class CategoriaResponseDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(80)]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; }
    }
}