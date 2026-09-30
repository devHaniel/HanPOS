using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Entities
{
    public class Compra : IEntity
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Total { get; set; }

        public EstadoCompra Estado { get; set; }

        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; } = null!;

        public int ProveedorId { get; set; }

        public Proveedor Proveedor { get; set; } = null!;

        public int CajaId {get; set;}
        public Caja Caja {get; set;}

        public ICollection<DetalleCompra> Detalles { get; set; }
            = new List<DetalleCompra>();
    }

    public enum EstadoCompra
    {
        Completada = 1,
        Pendiente = 2,
        Cancelada = 3
    }
}