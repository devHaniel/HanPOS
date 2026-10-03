using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Entities
{
    public class Categoria : IEntity
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<Producto> Productos { get; set; }
            = new List<Producto>();
    }
}