using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.DTOs.Compra;
using Api.DTOs.DetalleCompra;
using Api.DTOs.Paginacion;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;

namespace Api.Services.Implementations
{
    public class CompraService : ICompraService
    {
        private readonly ICompraRepository _compraRepository;
        private readonly IDetalleCompraRepository _detalleRepository;
        private readonly IProductoRepository _productoRepostory;
        private readonly IMovimientoCajaRepository _movimientoRepository;
        private readonly ICajaRepository _cajaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompraService(
            ICompraRepository compraRepository,
            IUnitOfWork unitOfWork,
            IDetalleCompraRepository detalleRepository,
            IProductoRepository productoRepository,
            IMovimientoCajaRepository movimientoCajaRepository,
            ICajaRepository cajaRepository)
        {
            _compraRepository = compraRepository;
            _unitOfWork = unitOfWork;
            _detalleRepository = detalleRepository;
            _productoRepostory = productoRepository;
            _cajaRepository = cajaRepository;
            _movimientoRepository = movimientoCajaRepository;
        }

        public async Task<IReadOnlyList<CompraResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var compras = await _compraRepository.GetAllAsync(cancellationToken);
            return compras.Select(MapToDTO).ToList().AsReadOnly();
        }

        public async Task<PagedResult<CompraResponseDto>> GetAllPagedAsync(int pagina = 1, int cantidad = 10, CancellationToken cancellationToken = default)
        {
            var (items, total) = await _compraRepository.GetAllPagedAsync(pagina, cantidad, cancellationToken);
            return new PagedResult<CompraResponseDto>
            {
                Items = items.Select(MapToDTO).ToList(),
                Pagina = pagina,
                Cantidad = cantidad,
                Total = total
            };
        }

        public async Task<CompraResponseDto?> GetByIdAsync(int compraId, CancellationToken cancellationToken = default)
        {
            var compra = await _compraRepository.GetWithDetailsAndProveedorAsync(compraId, cancellationToken);
            return compra != null ? MapToDTO(compra) : null;
        }

        public async Task<IReadOnlyList<CompraResponseDto>> GetPorFechaAsync(DateTime fecha, CancellationToken cancellationToken = default)
        {
            var all = await _compraRepository.GetAllAsync(cancellationToken);
            return all.Where(c => c.Fecha.Date == fecha.Date).Select(MapToDTO).ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<CompraResponseDto>> GetPorProveedorAsync(int proveedorId, CancellationToken cancellationToken = default)
        {
            var all = await _compraRepository.GetAllAsync(cancellationToken);
            return all.Where(c => c.ProveedorId == proveedorId).Select(MapToDTO).ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<DetalleCompraResponseDto>> GetDetallesAsync(int compraId, CancellationToken cancellationToken = default)
        {
            var detalles = await _detalleRepository.GetByCompraIdAsync(compraId, cancellationToken);
            return detalles.Select(MapToDetalleDTO).ToList().AsReadOnly();
        }

        public async Task<CompraResponseDto> CrearAsync(CompraCrearRequest dto, CancellationToken cancellationToken = default)
        {
            // Mapear DTO a entidad
            var compra = new Compra
            {
                Fecha = dto.Fecha,
                Estado = (EstadoCompra)dto.Estado,
                UsuarioId = dto.UsuarioId,
                ProveedorId = dto.ProveedorId,
                CajaId = dto.CajaId,
                Detalles = new List<DetalleCompra>()
            };

            // Mapear detalles
            if (dto.Detalles != null)
            {
                foreach (var d in dto.Detalles)
                {
                    compra.Detalles.Add(new DetalleCompra
                    {
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad
                    });
                }
            }

            // Validar y procesar la compra
            await ValidarCompraAsync(compra, cancellationToken);
            await ProcesarDetallesAsync(compra, cancellationToken);
            CalcularTotales(compra);

            await _compraRepository.AddAsync(compra, cancellationToken);
            await RegistrarMovimientoCajaAsync(compra, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Recargar con detalles para el DTO de respuesta
            var compraCompleta = await _compraRepository.GetWithDetailsAndProveedorAsync(compra.Id, cancellationToken);
            return MapToDTO(compraCompleta!);
        }

        public async Task<CompraResponseDto?> ActualizarAsync(CompraActualizarRequest dto, CancellationToken cancellationToken = default)
        {
            var existente = await _compraRepository.GetWithDetailsAndProveedorAsync(dto.Id, cancellationToken);
            if (existente == null) return null;

            // Actualizar campos principales
            existente.Fecha = dto.Fecha;
            existente.Estado = (EstadoCompra)dto.Estado;
            existente.UsuarioId = dto.UsuarioId;
            existente.CajaId = dto.CajaId;
            existente.ProveedorId = dto.ProveedorId;

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
                    existente.Detalles.Add(new DetalleCompra
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
            var compra = await _compraRepository.GetWithDetailsAndProveedorAsync(id, cancellationToken);
            if (compra == null) return false;

            await _compraRepository.RemoveAsync(compra, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IReadOnlyList<CompraResponseDto>> GetPorCajaAsync(int cajaId, CancellationToken cancellationToken = default)
        {
            var all = await _compraRepository.GetAllAsync(cancellationToken);
            return all.Where(c => c.CajaId == cajaId).Select(MapToDTO).ToList().AsReadOnly();
        }

        private async Task ValidarCompraAsync(Compra compra, CancellationToken cancellationToken)
        {
            if (compra.Detalles == null || !compra.Detalles.Any())
                throw new Exception("La compra debe contener al menos un producto.");

            if (compra.Detalles.Any(x => x.Cantidad <= 0))
                throw new Exception("La cantidad debe ser mayor a cero.");

            if (compra.Detalles.Any(x => x.ProductoId <= 0))
                throw new Exception("La compra contiene un producto inválido.");
        }

        private async Task RegistrarMovimientoCajaAsync(Compra compra, CancellationToken cancellationToken)
        {
            var caja = await _cajaRepository.GetAbiertaAsync(cancellationToken);

            if(caja == null || caja.Id != compra.CajaId)
                throw new Exception("La caja asociada a la compra no está abierta o no coincide con la compra.");

            var movimiento = new MovimientoCaja
            {
                CajaId = compra.CajaId,
                Monto = compra.Total,
                Tipo = TipoMovimiento.Egreso,
                Concepto = $"Compra #{compra.Id}"
            };

            await _movimientoRepository.AddAsync(movimiento, cancellationToken);
        }

        private async Task ProcesarDetallesAsync(Compra compra, CancellationToken cancellationToken)
        {
            foreach (var detalle in compra.Detalles)
            {
                var producto = await _productoRepostory.GetByIdAsync(detalle.ProductoId, cancellationToken);

                if (producto == null)
                    throw new Exception($"El producto {detalle.ProductoId} no existe.");

                detalle.PrecioUnitario = producto.PrecioCompra;
                producto.Stock += detalle.Cantidad; // En compra se suma al stock
            }
        }

        private void CalcularTotales(Compra compra)
        {
            decimal subtotal = 0;

            foreach (var detalle in compra.Detalles)
            {
                detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;
                subtotal += detalle.Subtotal;
            }

            compra.Subtotal = subtotal;
            compra.Impuesto = subtotal * 0.15m;
            compra.Total = compra.Subtotal + compra.Impuesto;
        }

        private static CompraResponseDto MapToDTO(Compra compra)
        {
            return new CompraResponseDto
            {
                Id = compra.Id,
                Fecha = compra.Fecha,
                Subtotal = compra.Subtotal,
                Impuesto = compra.Impuesto,
                Total = compra.Total,
                Estado = (int)compra.Estado,
                UsuarioId = compra.UsuarioId,
                CajaId = compra.CajaId,
                ProveedorId = compra.ProveedorId
            };
        }

        private static DetalleCompraResponseDto MapToDetalleDTO(DetalleCompra detalle)
        {
            return new DetalleCompraResponseDto
            {
                Id = detalle.Id,
                CompraId = detalle.CompraId,
                ProductoId = detalle.ProductoId,
                Cantidad = detalle.Cantidad,
                PrecioUnitario = detalle.PrecioUnitario,
                Subtotal = detalle.Subtotal
            };
        }
    }
}