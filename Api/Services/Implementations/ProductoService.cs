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
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductoService(
            IProductoRepository productoRepository, 
            IUnitOfWork unitOfWork,
            ICategoriaRepository categoriaRepository)
        {
            _productoRepository = productoRepository;
            _unitOfWork = unitOfWork;
            _categoriaRepository = categoriaRepository;
        }

        public async Task<IReadOnlyList<Producto>> GetActivosAsync(CancellationToken cancellationToken = default)
        {
            return await _productoRepository.GetActivosAsync(cancellationToken);
        }

        public async Task<Producto> CrearAsync(Producto producto, CancellationToken cancellationToken = default)
        {
            var codigoExiste = await _productoRepository.GetByCodigo(producto.Codigo);
            var categoriaExiste = await _categoriaRepository.GetById(producto.CategoriaId, cancellationToken);

            if(categoriaExiste == null)
                throw new ArgumentNullException("Categoría no encontrada.");
            if(codigoExiste != null)
                throw new ArgumentNullException("Ya existe un producto con el mismo codigo.");
            if(producto.PrecioVenta < 0)
                throw new ArgumentException("El precio debe ser mayor a 0.");
            if(producto.Stock < 0)
                throw new ArgumentException("El stock inicial debe ser igual o mayor a 0.");

            await _productoRepository.AddAsync(producto, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return producto;
        }

        public async Task<bool> ActualizarAsync(Producto producto, CancellationToken cancellationToken = default)
        {
            var existente = await _productoRepository.GetActivosAsync(cancellationToken);
            if (existente == null || existente.Count == 0) return false;
            var first = existente.First();

            if(producto.PrecioVenta < 0)
                throw new ArgumentException("El precio de venta debe ser mayor a 0.");
            if(producto.PrecioCompra < 0)
                throw new ArgumentException("El precio de compra debe ser mayor a 0.");
            if(producto.Stock < 0)
                throw new ArgumentException("El stock inicial debe ser igual o mayor a 0.");
            var categoriaExiste = await _categoriaRepository.GetById(producto.CategoriaId, cancellationToken);

            if(categoriaExiste == null)
                throw new ArgumentNullException("Categoría no encontrada.");

            first.Nombre = producto.Nombre;
            first.PrecioVenta = producto.PrecioVenta;
            first.PrecioCompra = producto.PrecioCompra;
            first.Stock = producto.Stock;
            first.CategoriaId = producto.CategoriaId;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
        {
            var producto = await _productoRepository.GetActivosAsync(cancellationToken);
            var eliminar = producto.FirstOrDefault(p => p.Id == id);
            if (eliminar == null) return false;

            await _productoRepository.RemoveAsync(eliminar, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}