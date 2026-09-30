using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Entities
{
    public class Caja : IEntity
    {
        public int Id { get; set; }

        public DateTime FechaApertura { get; set; }

        public DateTime? FechaCierre { get; set; }

        public decimal MontoInicial { get; set; }

        public decimal? MontoFinal { get; set; }

        public bool EstaAbierta => FechaCierre == null;

        // Usuario que abrió la caja
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        // Movimientos de dinero de esta caja
        public ICollection<MovimientoCaja> Movimientos { get; set; }
            = new List<MovimientoCaja>();
    }
}