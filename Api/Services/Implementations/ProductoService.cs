using Api.DTOs.Producto;
using Api.DTOs.DetalleVenta;
using Api.DTOs.DetalleCompra;
using Api.DTOs.Paginacion;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Implementations
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IDetalleVentaRepository _detalleVentaRepository;
        private readonly IDetalleCompraRepository _detalleCompraRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductoService(
            IProductoRepository productoRepository,
            IUnitOfWork unitOfWork,
            ICategoriaRepository categoriaRepository,
            IDetalleVentaRepository detalleVentaRepository,
            IDetalleCompraRepository detalleCompraRepository)
        {
            _productoRepository = productoRepository;
            _unitOfWork = unitOfWork;
            _categoriaRepository = categoriaRepository;
            _detalleVentaRepository = detalleVentaRepository;
            _detalleCompraRepository = detalleCompraRepository;
        }

        public async Task<IReadOnlyList<ProductoResponseDto>> GetActivosAsync(CancellationToken cancellationToken = default)
        {
            var productos = await _productoRepository.GetActivosAsync(cancellationToken);
            return productos.Select(MapToDTO).ToList().AsReadOnly();
        }

        public async Task<PagedResult<ProductoResponseDto>> GetActivosPagedAsync(int pagina = 1, int cantidad = 10, CancellationToken cancellationToken = default)
        {
            var (items, total) = await _productoRepository.GetActivosPagedAsync(pagina, cantidad, cancellationToken);
            return new PagedResult<ProductoResponseDto>
            {
                Items = items.Select(MapToDTO).ToList(),
                Pagina = pagina,
                Cantidad = cantidad,
                Total = total
            };
        }

        public async Task<ProductoResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var producto = await _productoRepository.GetByIdAsync(id, cancellationToken);
            return producto != null ? MapToDTO(producto) : null;
        }

        public async Task<ProductoResponseDto?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
        {
            var producto = await _productoRepository.GetByCodigoAsync(codigo, cancellationToken);
            return producto != null ? MapToDTO(producto) : null;
        }

        public async Task<IReadOnlyList<DetalleVentaResponseDto>> GetVentasAsync(int productoId, CancellationToken cancellationToken = default)
        {
            var detalles = await _detalleVentaRepository.GetByProductoIdAsync(productoId, cancellationToken);
            return detalles.Select(MapToDetalleVentaDTO).ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<DetalleCompraResponseDto>> GetComprasAsync(int productoId, CancellationToken cancellationToken = default)
        {
            var detalles = await _detalleCompraRepository.GetByProductoIdAsync(productoId, cancellationToken);
            return detalles.Select(MapToDetalleCompraDTO).ToList().AsReadOnly();
        }

        public async Task<ProductoResponseDto> CrearAsync(ProductoCrearRequest dto, CancellationToken cancellationToken = default)
        {
            // Validaciones de negocio
            var codigoExiste = await _productoRepository.GetByCodigoAsync(dto.Codigo, cancellationToken);
            if (codigoExiste != null)
                throw new ArgumentException("Ya existe un producto con el mismo código.");

            if (dto.CategoriaId.HasValue)
            {
                var categoriaExiste = await _categoriaRepository.GetByIdAsync(dto.CategoriaId.Value, cancellationToken);
                if (categoriaExiste == null)
                    throw new ArgumentException("Categoría no encontrada.");
            }

            if (dto.PrecioVenta < 0)
                throw new ArgumentException("El precio de venta debe ser mayor o igual a 0.");
            if (dto.PrecioCompra < 0)
                throw new ArgumentException("El precio de compra debe ser mayor o igual a 0.");
            if (dto.Stock < 0)
                throw new ArgumentException("El stock inicial debe ser mayor o igual a 0.");

            var producto = new Producto
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                PrecioVenta = dto.PrecioVenta,
                PrecioCompra = dto.PrecioCompra,
                Stock = dto.Stock,
                StockMinimo = dto.StockMinimo,
                Activo = dto.Activo,
                CategoriaId = dto.CategoriaId ?? 0
            };

            await _productoRepository.AddAsync(producto, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(producto);
        }

        public async Task<ProductoResponseDto?> ActualizarAsync(ProductoActualizarRequest dto, CancellationToken cancellationToken = default)
        {
            var existente = await _productoRepository.GetByIdAsync(dto.Id, cancellationToken);
            if (existente == null) return null;

            // Validar código único (excluyendo el actual)
            var codigoExiste = await _productoRepository.GetByCodigoAsync(dto.Codigo, cancellationToken);
            if (codigoExiste != null && codigoExiste.Id != dto.Id)
                throw new ArgumentException("Ya existe un producto con el mismo código.");

            if (dto.CategoriaId.HasValue)
            {
                var categoriaExiste = await _categoriaRepository.GetByIdAsync(dto.CategoriaId.Value, cancellationToken);
                if (categoriaExiste == null)
                    throw new ArgumentException("Categoría no encontrada.");
            }

            if (dto.PrecioVenta < 0)
                throw new ArgumentException("El precio de venta debe ser mayor o igual a 0.");
            if (dto.PrecioCompra < 0)
                throw new ArgumentException("El precio de compra debe ser mayor o igual a 0.");
            if (dto.Stock < 0)
                throw new ArgumentException("El stock debe ser mayor o igual a 0.");

            existente.Codigo = dto.Codigo;
            existente.Nombre = dto.Nombre;
            existente.PrecioVenta = dto.PrecioVenta;
            existente.PrecioCompra = dto.PrecioCompra;
            existente.Stock = dto.Stock;
            existente.StockMinimo = dto.StockMinimo;
            existente.Activo = dto.Activo;
            existente.CategoriaId = dto.CategoriaId ?? 0;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(existente);
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var producto = await _productoRepository.GetByIdAsync(id, cancellationToken);
            if (producto == null) return false;

            producto.Activo = false; // Soft delete
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static ProductoResponseDto MapToDTO(Producto producto)
        {
            return new ProductoResponseDto
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                PrecioVenta = producto.PrecioVenta,
                PrecioCompra = producto.PrecioCompra,
                Stock = producto.Stock,
                StockMinimo = producto.StockMinimo,
                Activo = producto.Activo,
                CategoriaId = producto.CategoriaId
            };
        }

        private static DetalleVentaResponseDto MapToDetalleVentaDTO(DetalleVenta detalle)
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

        private static DetalleCompraResponseDto MapToDetalleCompraDTO(DetalleCompra detalle)
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