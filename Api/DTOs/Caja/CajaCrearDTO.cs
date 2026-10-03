using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs.Caja
{
    public class CajaCrearDTO
    {

        public DateTime FechaApertura { get; set; } = DateTime.UtcNow;

        public decimal MontoInicial { get; set; } = 0;

        public decimal? MontoFinal { get; set; } = 0;

        public bool? EstaAbierta = true;
        public int UsuarioId { get; set; }
    }
}