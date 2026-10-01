using System;

namespace Api.DTOs.Caja
{
    /// <summary>
    /// DTO para representar una caja. Contiene únicamente los datos propios de la entidad Caja.
    /// Las relaciones (Usuario, Movimientos) se consultan mediante endpoints específicos.
    /// </summary>
    public class CajaResponseDto
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