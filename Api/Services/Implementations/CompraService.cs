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
    public class CompraService : ICompraService
    {
        private readonly ICompraRepository _compraRepository;
        private readonly IDetalleVentaRepository _detalleRepository;
        private readonly IProductoRepository _productoRepostory;
        private readonly IMovimientoCajaRepository _movimientoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompraService(
            ICompraRepository compraRepository, 
            IUnitOfWork unitOfWork,
            IDetalleVentaRepository detalleRepository,
            IProductoRepository productoRepository,
            IMovimientoCajaRepository movimientoCajaRepository)
        {
            _compraRepository = compraRepository;
            _unitOfWork = unitOfWork;
            _detalleRepository = detalleRepository;
            _productoRepostory = productoRepository;
            _movimientoRepository = movimientoCajaRepository;

        }

        public async Task<IReadOnlyList<Compra>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _compraRepository.GetAllAsync(cancellationToken);
        }

        public async Task<Compra?> GetWithDetailsAndProveedorAsync(int compraId, CancellationToken cancellationToken = default)
        {
            return await _compraRepository.GetWithDetailsAndProveedorAsync(compraId, cancellationToken);
        }

        public async Task<IReadOnlyList<Compra>> GetPorFechaAsync(DateTime fecha, CancellationToken cancellationToken = default)
        {
            var all = await _compraRepository.GetAllAsync(cancellationToken);
            return all.Where(c => c.Fecha.Date == fecha.Date).ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<Compra>> GetPorProveedorAsync(int proveedorId, CancellationToken cancellationToken = default)
        {
            var all = await _compraRepository.GetAllAsync(cancellationToken);
            return all.Where(c => c.ProveedorId == proveedorId).ToList().AsReadOnly();
        }

        public async Task<Compra> CrearAsync(Compra compra, CancellationToken cancellationToken = default)
        {
            await ValidarCompraAsync(compra, cancellationToken);

            await ProcesarDetallesAsync(compra, cancellationToken);

            CalcularTotales(compra);

            await _compraRepository.AddAsync(compra, cancellationToken);

            await RegistrarMovimientoCajaAsync(compra, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return compra;
        }

        public async Task<bool> ActualizarAsync(Compra compra, CancellationToken cancellationToken = default)
        {
            var existente = await _compraRepository.GetWithDetailsAndProveedorAsync(compra.Id, cancellationToken);
            if (existente == null) return false;

            existente.ProveedorId = compra.ProveedorId;
            existente.Fecha = compra.Fecha;
            existente.Total = compra.Total;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var compra = await _compraRepository.GetWithDetailsAndProveedorAsync(id, cancellationToken);
            if (compra == null) return false;

            await _compraRepository.RemoveAsync(compra, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task ValidarCompraAsync(
            Compra compra,
            CancellationToken cancellationToken)
        {
            if (compra.Detalles == null || !compra.Detalles.Any())
                throw new Exception("La compra debe contener al menos un producto.");

            if (compra.Detalles.Any(x => x.Cantidad <= 0))
                throw new Exception("La cantidad debe ser mayor a cero.");

            if (compra.Detalles.Any(x => x.ProductoId <= 0))
                throw new Exception("La compra contiene un producto inválido.");

            // Aquí después podrías validar:
            // - Caja abierta
            // - Usuario válido
            // - Cliente válido
            // - etc.

        }

        private async Task RegistrarMovimientoCajaAsync(Compra compra, CancellationToken cancellationToken)
        {
            // Si la venta fue en efectivo esta tocando la caja
            // por ende es un movimiento de caja
            var movimiento = new MovimientoCaja
            {
                CajaId = compra.CajaId,
                Monto = compra.Total,
                Tipo = TipoMovimiento.Egreso,
                Concepto = $"Compra"
            };

            await _movimientoRepository.AddAsync(movimiento, cancellationToken);
        }

        private async Task ProcesarDetallesAsync(Compra compra, CancellationToken cancellationToken)
        {
            
            foreach(var detalle in compra.Detalles)
            {
                var producto = await _productoRepostory
                    .GetById(detalle.Id, cancellationToken);

                if (producto == null)
                    throw new Exception(
                        $"El producto {detalle.ProductoId} no existe.");

                if (producto.Stock < detalle.Cantidad)
                    throw new Exception(
                        $"Stock insuficiente para {producto.Nombre}.");

                detalle.PrecioUnitario = producto.PrecioCompra;

                producto.Stock -= detalle.Cantidad;

            }

            // productos
            // stock
            // crear/modificar detalles
        }

        private void CalcularTotales(Compra compra)
        {
            decimal subtotal = 0;

            foreach (var detalle in compra.Detalles)
            {
                detalle.Subtotal =
                    detalle.Cantidad * detalle.PrecioUnitario;

                subtotal += detalle.Subtotal;
            }

            compra.Subtotal = subtotal;

            compra.Impuesto = subtotal * 0.15m;

            compra.Total = compra.Subtotal + compra.Impuesto;
        }
    }
}