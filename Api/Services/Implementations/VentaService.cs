using Api.Data;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Implementations
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IDetalleVentaRepository _detalleRepository;
        private readonly IProductoRepository _productoRepostory;
        private readonly IMovimientoCajaRepository _movimientoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VentaService(
            IVentaRepository ventaRepository, 
            IUnitOfWork unitOfWork,
            IDetalleVentaRepository detalleRepository,
            IProductoRepository productoRepository,
            IMovimientoCajaRepository movimientoCajaRepository)
        {
            _ventaRepository = ventaRepository;
            _unitOfWork = unitOfWork;
            _detalleRepository = detalleRepository;
            _productoRepostory = productoRepository;
            _movimientoRepository = movimientoCajaRepository;
        }

        public async Task<IReadOnlyList<Venta>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _ventaRepository.GetAllAsync(cancellationToken);
        }

        public async Task<Venta?> GetWithDetailsAndClienteAsync(int ventaId, CancellationToken cancellationToken = default)
        {
            return await _ventaRepository.GetWithDetailsAndClienteAsync(ventaId, cancellationToken);
        }

        public async Task<IReadOnlyList<Venta>> GetPorFechaAsync(DateTime fecha, CancellationToken cancellationToken = default)
        {
            var all = await _ventaRepository.GetAllAsync(cancellationToken);
            return all.Where(v => v.Fecha.Date == fecha.Date).ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<Venta>> GetPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default)
        {
            var all = await _ventaRepository.GetAllAsync(cancellationToken);
            return all.Where(v => v.UsuarioId == usuarioId).ToList().AsReadOnly();
        }

        public async Task<Venta> CrearAsync(Venta venta, CancellationToken cancellationToken = default)
        {
            // Venta tendra muchas partes, validaciones etc...
            // Serán modularizadas para tener mayor facilidad
            // Se utilizan detallesVentas y a la vez Productos 
            // Para ir creando tanto la venta como sus detalles
            // Y verificar que el stock del producto sea valido

            await ValidarVentaAsync(venta, cancellationToken);

            await ProcesarDetallesAsync(venta, cancellationToken);

            CalcularTotales(venta);

            await _ventaRepository.AddAsync(venta, cancellationToken);

            await RegistrarMovimientoCajaAsync(venta, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return venta;
        }

        public async Task<bool> ActualizarAsync(Venta venta, CancellationToken cancellationToken = default)
        {
            // var existente = await _ventaRepository.GetWithDetailsAndClienteAsync(venta.Id, cancellationToken);
            // if (existente == null) return false;

            // existente.UsuarioId = venta.UsuarioId;
            // existente.FechaVenta = venta.FechaVenta;
            // existente.MontoTotal = venta.MontoTotal;
            // await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var venta = await _ventaRepository.GetWithDetailsAndClienteAsync(id, cancellationToken);
            if (venta == null) return false;

            await _ventaRepository.RemoveAsync(venta, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task ValidarVentaAsync(
            Venta venta,
            CancellationToken cancellationToken)
        {
            if (venta.Detalles == null || !venta.Detalles.Any())
                throw new Exception("La venta debe contener al menos un producto.");

            if (venta.Detalles.Any(x => x.Cantidad <= 0))
                throw new Exception("La cantidad debe ser mayor a cero.");

            if (venta.Detalles.Any(x => x.ProductoId <= 0))
                throw new Exception("La venta contiene un producto inválido.");

            // Aquí después podrías validar:
            // - Caja abierta
            // - Usuario válido
            // - Cliente válido
            // - etc.

        }

        private async Task RegistrarMovimientoCajaAsync(Venta venta, CancellationToken cancellationToken)
        {
            // Si la venta fue en efectivo esta tocando la caja
            // por ende es un movimiento de caja
            if (venta.MetodoPago != MetodoPago.Efectivo)
                return;
            var movimiento = new MovimientoCaja
            {
                CajaId = venta.CajaId,
                Monto = venta.Total,
                Tipo = TipoMovimiento.Ingreso,
                Concepto = $"Venta"
            };

            await _movimientoRepository.AddAsync(movimiento, cancellationToken);
        }

        private async Task ProcesarDetallesAsync(Venta venta, CancellationToken cancellationToken)
        {
            
            foreach(var detalle in venta.Detalles)
            {
                var producto = await _productoRepostory
                    .GetById(detalle.Id, cancellationToken);

                if (producto == null)
                    throw new Exception(
                        $"El producto {detalle.ProductoId} no existe.");

                if (producto.Stock < detalle.Cantidad)
                    throw new Exception(
                        $"Stock insuficiente para {producto.Nombre}.");

                detalle.PrecioUnitario = producto.PrecioVenta;

                producto.Stock -= detalle.Cantidad;

            }

            // productos
            // stock
            // crear/modificar detalles
        }

        private void CalcularTotales(Venta venta)
        {
            decimal subtotal = 0;

            foreach (var detalle in venta.Detalles)
            {
                detalle.Subtotal =
                    detalle.Cantidad * detalle.PrecioUnitario;

                subtotal += detalle.Subtotal;
            }

            venta.Subtotal = subtotal;

            venta.Impuesto = subtotal * 0.15m;

            venta.Total = venta.Subtotal + venta.Impuesto;
        }
    }
}