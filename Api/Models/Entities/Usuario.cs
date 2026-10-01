using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Api.Models.Entities
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public class Usuario : IdentityUser<int> 
    {
        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; } = true;

        public ICollection<Caja> Cajas { get; set; } = new List<Caja>();

        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();

        public ICollection<Compra> Compras { get; set; } = new List<Compra>();
    }
}