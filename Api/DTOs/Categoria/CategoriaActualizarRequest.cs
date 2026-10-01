using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Categoria
{
    /// <summary>
    /// DTO para actualizar una categoría existente (request).
    /// </summary>
    public class CategoriaActualizarRequest
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(80)]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; } = true;
    }
}