using System;
using System.Collections.Generic;

namespace Api.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new Cliente entity.
    /// Used when creating a new cliente via API.
    /// </summary>
    public class ClienteCrearDTO
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;

        [MaxLength(20)]
        public string? Telefono { get; set; }

        [MaxLength(25)]
        public string? RTN { get; set; }
    }
}