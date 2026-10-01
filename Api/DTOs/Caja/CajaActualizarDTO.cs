using System;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Caja
{
    /// <summary>
    /// Data Transfer Object for updating a Caja entity.
    /// Used when updating an existing caja via API.
    /// </summary>
    public class CajaActualizarDTO
    {
        public int Id { get; set; }

        public DateTime FechaApertura { get; set; }

        public DateTime? FechaCierre { get; set; }

        public decimal MontoInicial { get; set; }

        public decimal? MontoFinal { get; set; }

        public int UsuarioId { get; set; }
    }
}