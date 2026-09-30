using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Entities
{
    public class Venta : IEntity
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Total { get; set; }

        public MetodoPago MetodoPago { get; set; }

        public EstadoVenta Estado { get; set; }

        public int CajaId {get; set;}
        public Caja Caja {get; set;}

        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; } = null!;

        public int? ClienteId { get; set; }

        public Cliente? Cliente { get; set; }

        public ICollection<DetalleVenta> Detalles { get; set; }
            = new List<DetalleVenta>();
    }

    public enum MetodoPago
    {
        Efectivo = 1,
        Tarjeta = 2,
        Transferencia = 3
    }

    public enum EstadoVenta
    {
        Completada = 1,
        Cancelada = 2
    }
}