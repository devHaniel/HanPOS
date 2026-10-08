using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de Producto.
    /// Define el contrato para operaciones sobre la entidad Producto.
    /// </summary>
    public interface IProductoRepository
    {
        // Obtener todos los productos activos
        Task<IReadOnlyList<Producto>> GetActivosAsync(
            CancellationToken cancellationToken = default);

        // Obtener todos los productos activos con paginación
        Task<(List<Producto> Items, int Total)> GetActivosPagedAsync(
            int pagina,
            int cantidad,
            CancellationToken cancellationToken = default);

        // Buscar productos por nombre o código con paginación
        Task<(List<Producto> Items, int Total)> BuscarPagedAsync(
            string? termino,
            int pagina,
            int cantidad,
            CancellationToken cancellationToken = default);

        // Obtener producto por ID
        Task<Producto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        // Obtener por código
        Task<Producto?> GetByCodigoAsync(
            string codigo,
            CancellationToken cancellationToken = default);

        // Obtener por categoría
        Task<IReadOnlyList<Producto>> GetPorCategoriaAsync(
            int categoriaId,
            CancellationToken cancellationToken = default);

        // Productos con stock bajo
        Task<IReadOnlyList<Producto>> StockBajoAsync(
            CancellationToken cancellationToken = default);

        // Buscar por nombre
        Task<IReadOnlyList<Producto>> BuscarPorNombreAsync(
            string termino,
            CancellationToken cancellationToken = default);

        // Agregar producto
        Task<Producto> AddAsync(
            Producto producto,
            CancellationToken cancellationToken = default);

        // Eliminar producto
        Task RemoveAsync(
            Producto producto,
            CancellationToken cancellationToken = default);
    }
}