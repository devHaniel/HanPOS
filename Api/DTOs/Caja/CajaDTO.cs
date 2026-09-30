using System;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Caja
{
    /// <summary>
    /// Data Transfer Object for Caja entity.
    /// Used for transferring caja data between layers.
    /// </summary>
    public class CajaDTO
    {
        public int Id { get; set; }

        public DateTime FechaApertura { get; set; }

        public DateTime? FechaCierre { get; set; }

        public decimal MontoInicial { get; set; }

        public decimal? MontoFinal { get; set; }

        public bool EstaAbierta => FechaCierre == null;

        public int UsuarioId { get; set; }

    }
}