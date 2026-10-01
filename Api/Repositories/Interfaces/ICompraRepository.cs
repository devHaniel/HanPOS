using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de Compra.
    /// Define el contrato para operaciones sobre la entidad Compra.
    /// </summary>
    public interface ICompraRepository
    {
        // Obtener todas las compras
        Task<IReadOnlyList<Compra>> GetAllAsync(
            CancellationToken cancellationToken = default);

        // Obtener todas las compras con paginación
        Task<(List<Compra> Items, int Total)> GetAllPagedAsync(
            int pagina,
            int cantidad,
            CancellationToken cancellationToken = default);

        // Obtener compra por ID
        Task<Compra?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        // Obtener compra con detalles y proveedor
        Task<Compra?> GetWithDetailsAndProveedorAsync(
            int compraId,
            CancellationToken cancellationToken = default);

        // Agregar compra
        Task<Compra> AddAsync(
            Compra compra,
            CancellationToken cancellationToken = default);

        // Eliminar compra
        Task RemoveAsync(
            Compra compra,
            CancellationToken cancellationToken = default);
    }
}