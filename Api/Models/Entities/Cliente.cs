using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Entities
{
    public class Cliente : IEntity
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Telefono { get; set; }

        public string? RTN { get; set; }

        public ICollection<Venta> Ventas { get; set; }
            = new List<Venta>();
    }
}