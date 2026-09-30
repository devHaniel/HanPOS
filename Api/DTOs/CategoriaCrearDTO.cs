using System;
using System.Collections.Generic;

namespace Api.DTOs.Categoria
{
    /// <summary>
    /// Data Transfer Object for creating a new Categoria entity.
    /// Used when creating a new categoria via API.
    /// </summary>
    public class CategoriaCrearDTO
    {
        [Required]
        [MaxLength(80)]
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; } = true;
    }
}