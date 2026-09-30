using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Entities
{
    public class Producto : IEntity
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public decimal PrecioVenta { get; set; }

        public decimal PrecioCompra { get; set; }

        public decimal Stock { get; set; }

        public decimal StockMinimo { get; set; }

        public bool Activo { get; set; } = true;

        public int CategoriaId { get; set; }

        public Categoria Categoria { get; set; } = null!;

        public ICollection<DetalleVenta> DetallesVenta { get; set; }
            = new List<DetalleVenta>();

        public ICollection<DetalleCompra> DetallesCompra { get; set; }
            = new List<DetalleCompra>();
    }
}