using System;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Caja
{
    /// <summary>
    /// DTO para crear una nueva caja / abrir sesión de caja (request).
    /// </summary>
    public class CajaCrearRequest
    {
        public DateTime FechaApertura { get; set; } = DateTime.Now;

        [Range(0, double.MaxValue)]
        public decimal MontoInicial { get; set; } = 0;

        [Required]
        public int UsuarioId { get; set; }
    }
}