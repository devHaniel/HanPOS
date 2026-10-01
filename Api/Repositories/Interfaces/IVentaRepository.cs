using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de Venta.
    /// Define el contrato para operaciones sobre la entidad Venta.
    /// </summary>
    public interface IVentaRepository
    {
        // Obtener todas las ventas
        Task<IReadOnlyList<Venta>> GetAllAsync(
            CancellationToken cancellationToken = default);

        // Obtener todas las ventas con paginación
        Task<(List<Venta> Items, int Total)> GetAllPagedAsync(
            int pagina,
            int cantidad,
            CancellationToken cancellationToken = default);

        // Obtener venta por ID
        Task<Venta?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        // Obtener venta con detalles y cliente
        Task<Venta?> GetWithDetailsAndClienteAsync(
            int ventaId,
            CancellationToken cancellationToken = default);

        // Agregar venta
        Task<Venta> AddAsync(
            Venta venta,
            CancellationToken cancellationToken = default);

        // Eliminar venta
        Task RemoveAsync(
            Venta venta,
            CancellationToken cancellationToken = default);
    }
}