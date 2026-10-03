using Api.DTOs.Categoria;
using Api.DTOs.Producto;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Implementations
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoriaService(ICategoriaRepository categoriaRepository, IProductoRepository productoRepository, IUnitOfWork unitOfWork)
        {
            _categoriaRepository = categoriaRepository;
            _productoRepository = productoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<CategoriaResponseDto>> GetActivasAsync(CancellationToken cancellationToken = default)
        {
            var categorias = await _categoriaRepository.GetActivasAsync(cancellationToken);
            return categorias.Select(MapToDTO).ToList().AsReadOnly();
        }

        public async Task<CategoriaResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id, cancellationToken);
            return categoria != null ? MapToDTO(categoria) : null;
        }

        public async Task<IReadOnlyList<ProductoResponseDto>> GetProductosAsync(int categoriaId, CancellationToken cancellationToken = default)
        {
            var productos = await _productoRepository.GetPorCategoriaAsync(categoriaId, cancellationToken);
            return productos.Select(MapToProductoDTO).ToList().AsReadOnly();
        }

        public async Task<CategoriaResponseDto> CrearAsync(CategoriaCrearRequest dto, CancellationToken cancellationToken = default)
        {
            var categoria = new Categoria
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Activo = dto.Activo
            };

            await _categoriaRepository.AddAsync(categoria, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(categoria);
        }

        public async Task<CategoriaResponseDto?> ActualizarAsync(CategoriaActualizarRequest dto, CancellationToken cancellationToken = default)
        {
            var existente = await _categoriaRepository.GetByIdAsync(dto.Id, cancellationToken);
            if (existente == null) return null;

            existente.Nombre = dto.Nombre;
            existente.Descripcion = dto.Descripcion;
            existente.Activo = dto.Activo;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDTO(existente);
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id, cancellationToken);
            if (categoria == null) return false;

            categoria.Activo = false; // Soft delete
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static CategoriaResponseDto MapToDTO(Categoria categoria)
        {
            return new CategoriaResponseDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Activo = categoria.Activo
            };
        }

        private static ProductoResponseDto MapToProductoDTO(Producto producto)
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
    }
}