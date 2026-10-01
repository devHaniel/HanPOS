using System;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Caja
{
    /// <summary>
    /// DTO para actualizar una caja existente (request).
    /// Nota: MontoFinal y FechaCierre se manejan mediante el endpoint de cerrar caja.
    /// </summary>
    public class CajaActualizarRequest
    {
        public int Id { get; set; }

        public DateTime FechaApertura { get; set; }

        public DateTime? FechaCierre { get; set; }

        public decimal MontoInicial { get; set; }

        public decimal? MontoFinal { get; set; }

        public int UsuarioId { get; set; }
    }
}