using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Entities
{
    public class MovimientoCaja : IEntity
    {
        public int Id { get; set; }

        public decimal Monto { get; set; }

        public TipoMovimiento Tipo { get; set; }

        public string? Concepto { get; set; }

        public DateTime Fecha { get; set; }

        public int CajaId { get; set; }
        public Caja Caja { get; set; } = null!;
    }

    public enum TipoMovimiento
    {
        Ingreso = 1,
        Egreso = 2
    }
}