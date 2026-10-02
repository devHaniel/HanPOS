using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.DTOs.Venta;
using Api.DTOs.DetalleVenta;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using Api.DTOs.Paginacion;

namespace Api.Services.Implementations
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IDetalleVentaRepository _detalleRepository;
        private readonly IProductoRepository _productoRepostory;
        private readonly IMovimientoCajaRepository _movimientoRepository;
        private readonly ICajaRepository _cajaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VentaService(
            IVentaRepository ventaRepository,
            IUnitOfWork unitOfWork,
            IDetalleVentaRepository detalleRepository,
            IProductoRepository productoRepository,
            IMovimientoCajaRepository movimientoCajaRepository,
            ICajaRepository cajaRepository)
        {
            _ventaRepository = ventaRepository;
            _unitOfWork = unitOfWork;
            _detalleRepository = detalleRepository;
            _productoRepostory = productoRepository;
            _cajaRepository = cajaRepository;
            _movimientoRepository = movimientoCajaRepository;
        }

        public async Task<PagedResult<VentaResponseDto>> GetAllPagedAsync(int pagina = 1, int cantidad = 10, CancellationToken cancellationToken = default)
        {
            var (items, total) = await _ventaRepository.GetAllPagedAsync(pagina, cantidad, cancellationToken);
            return new PagedResult<VentaResponseDto>
            {
                Items = items.Select(MapToDTO).ToList(),
                Pagina = pagina,
                Cantidad = cantidad,
                Total = total
            };
        }

        public async Task<IReadOnlyList<VentaResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var ventas = await _ventaRepository.GetAllAsync(cancellationToken);
            return ventas.Select(MapToDTO).ToList().AsReadOnly();
        }

        public async Task<VentaResponseDto?> GetByIdAsync(int ventaId, CancellationToken cancellationToken = default)
        {
            var venta = await _ventaRepository.GetWithDetailsAndClienteAsync(ventaId, cancellationToken);
            return venta != null ? MapToDTO(venta) : null;
        }

        public async Task<IReadOnlyList<VentaResponseDto>> GetPorFechaAsync(DateTime fecha, CancellationToken cancellationToken = default)
        {
            var all = await _ventaRepository.GetAllAsync(cancellationToken);
            return all.Where(v => v.Fecha.Date == fecha.Date).Select(MapToDTO).ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<VentaResponseDto>> GetPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default)
        {
            var all = await _ventaRepository.GetAllAsync(cancellationToken);
            return all.Where(v => v.UsuarioId == usuarioId).Select(MapToDTO).ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<DetalleVentaResponseDto>> GetDetallesAsync(int ventaId, CancellationToken cancellationToken = default)
        {
            var detalles = await _detalleRepository.GetByVentaIdAsync(ventaId, cancellationToken);
            return detalles.Select(MapToDetalleDTO).ToList().AsReadOnly();
        }

        public async Task<VentaResponseDto> CrearAsync(VentaCrearRequest dto, CancellationToken cancellationToken = default)
        {
            // Mapear DTO a entidad
            var venta = new Venta
            {
                Fecha = dto.Fecha,
                MetodoPago = dto.MetodoPago.HasValue ? (MetodoPago)dto.MetodoPago.Value : MetodoPago.Efectivo,
                Estado = dto.Estado.HasValue ? (EstadoVenta)dto.Estado.Value : EstadoVenta.Completada,
                UsuarioId = dto.UsuarioId,
                CajaId = dto.CajaId,
                ClienteId = dto.ClienteId,
                Detalles = new List<DetalleVenta>()
            };
            
            // Ensure non-nullable enums have valid values
            if (!dto.MetodoPago.HasValue)
                venta.MetodoPago = MetodoPago.Efectivo;
            if (!dto.Estado.HasValue)
                venta.Estado = EstadoVenta.Completada;

            // Mapear detalles
            if (dto.Detalles != null)
            {
                foreach (var d in dto.Detalles)
                {
                    venta.Detalles.Add(new DetalleVenta
                    {
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad
                    });
                }
            }

            // Validar y procesar la venta
            await ValidarVentaAsync(venta, cancellationToken);
            await ProcesarDetallesAsync(venta, cancellationToken);
            CalcularTotales(venta);

            await _ventaRepository.AddAsync(venta, cancellationToken);
            await RegistrarMovimientoCajaAsync(venta, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Recargar para el DTO de respuesta
            var ventaCompleta = await _ventaRepository.GetWithDetailsAndClienteAsync(venta.Id, cancellationToken);
            return MapToDTO(ventaCompleta!);
        }

        public async Task<VentaResponseDto?> ActualizarAsync(VentaActualizarRequest dto, CancellationToken cancellationToken = default)
        {
            var existente = await _ventaRepository.GetWithDetailsAndClienteAsync(dto.Id, cancellationToken);
            if (existente == null) return null;

            // Actualizar campos principales
            existente.Fecha = dto.Fecha;
            if (dto.MetodoPago.HasValue)
                existente.MetodoPago = (MetodoPago)dto.MetodoPago.Value;
            if (dto.Estado.HasValue)
                existente.Estado = (EstadoVenta)dto.Estado.Value;
            existente.UsuarioId = dto.UsuarioId;
            existente.CajaId = dto.CajaId;
            existente.ClienteId = dto.ClienteId;

            // Actualizar detalles si se proporcionan
            if (dto.Detalles != null)
            {
                // Eliminar detalles existentes
                foreach (var detalle in existente.Detalles.ToList())
                {
                    await _detalleRepository.RemoveAsync(detalle, cancellationToken);
                }

                // Agregar nuevos detalles
                foreach (var d in dto.Detalles)
                {
                    existente.Detalles.Add(new DetalleVenta
                    {
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad
                    });
                }

                // Recalcular totales
                CalcularTotales(existente);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(existente);
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var venta = await _ventaRepository.GetWithDetailsAndClienteAsync(id, cancellationToken);
            if (venta == null) return false;

            await _ventaRepository.RemoveAsync(venta, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task ValidarVentaAsync(Venta venta, CancellationToken cancellationToken)
        {
            if (venta.Detalles == null || !venta.Detalles.Any())
                throw new Exception("La venta debe contener al menos un producto.");

            if (venta.Detalles.Any(x => x.Cantidad <= 0))
                throw new Exception("La cantidad debe ser mayor a cero.");

            if (venta.Detalles.Any(x => x.ProductoId <= 0))
                throw new Exception("La venta contiene un producto inválido.");
        }

        private async Task RegistrarMovimientoCajaAsync(Venta venta, CancellationToken cancellationToken)
        {
            var caja = await _cajaRepository.GetAbiertaAsync(cancellationToken);

            if(caja == null || caja.Id != venta.CajaId)
                throw new Exception("La caja asociada a la venta no está abierta o no coincide con la venta.");

            if (venta.MetodoPago != MetodoPago.Efectivo)
                return;

            var movimiento = new MovimientoCaja
            {
                CajaId = venta.CajaId,
                Monto = venta.Total,
                Tipo = TipoMovimiento.Ingreso,
                Concepto = $"Venta #{venta.Id}"
            };

            await _movimientoRepository.AddAsync(movimiento, cancellationToken);
        }

        private async Task ProcesarDetallesAsync(Venta venta, CancellationToken cancellationToken)
        {
            foreach (var detalle in venta.Detalles)
            {
                var producto = await _productoRepostory.GetByIdAsync(detalle.ProductoId, cancellationToken);

                if (producto == null)
                    throw new Exception($"El producto {detalle.ProductoId} no existe.");

                if (producto.Stock < detalle.Cantidad)
                    throw new Exception($"Stock insuficiente para {producto.Nombre}.");

                detalle.PrecioUnitario = producto.PrecioVenta;
                producto.Stock -= detalle.Cantidad;
            }
        }

        private void CalcularTotales(Venta venta)
        {
            decimal subtotal = 0;

            foreach (var detalle in venta.Detalles)
            {
                detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;
                subtotal += detalle.Subtotal;
            }

            venta.Subtotal = subtotal;
            venta.Impuesto = subtotal * 0.15m;
            venta.Total = venta.Subtotal + venta.Impuesto;
        }

        private static VentaResponseDto MapToDTO(Venta venta)
        {
            return new VentaResponseDto
            {
                Id = venta.Id,
                Fecha = venta.Fecha,
                Subtotal = venta.Subtotal,
                Impuesto = venta.Impuesto,
                Total = venta.Total,
                MetodoPago = (int?)venta.MetodoPago,
                Estado = (int?)venta.Estado,
                UsuarioId = venta.UsuarioId,
                CajaId = venta.CajaId,
                ClienteId = venta.ClienteId
            };
        }

        private static DetalleVentaResponseDto MapToDetalleDTO(DetalleVenta detalle)
        {
            return new DetalleVentaResponseDto
            {
                Id = detalle.Id,
                VentaId = detalle.VentaId,
                ProductoId = detalle.ProductoId,
                Cantidad = detalle.Cantidad,
                PrecioUnitario = detalle.PrecioUnitario,
                Subtotal = detalle.Subtotal
            };
        }
    }
}