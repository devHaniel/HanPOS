using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Categoria
{
    /// <summary>
    /// Data Transfer Object for updating a Categoria entity.
    /// Used when updating an existing categoria via API.
    /// </summary>
    public class CategoriaActualizarDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(80)]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; } = true;
    }
}