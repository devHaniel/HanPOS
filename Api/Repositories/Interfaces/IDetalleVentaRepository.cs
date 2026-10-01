using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de DetalleVenta.
    /// Define el contrato para operaciones sobre la entidad DetalleVenta.
    /// </summary>
    public interface IDetalleVentaRepository
    {
        // Obtener todos los detalles de venta
        Task<IReadOnlyList<DetalleVenta>> GetAllAsync(
            CancellationToken cancellationToken = default);

        // Obtener detalle por ID
        Task<DetalleVenta?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        // Obtener por venta
        Task<IReadOnlyList<DetalleVenta>> GetPorVentaAsync(
            int ventaId,
            CancellationToken cancellationToken = default);

        // Obtener por venta (alias)
        Task<IReadOnlyList<DetalleVenta>> GetByVentaIdAsync(
            int ventaId,
            CancellationToken cancellationToken = default);

        // Obtener por producto
        Task<IReadOnlyList<DetalleVenta>> GetByProductoIdAsync(
            int productoId,
            CancellationToken cancellationToken = default);

        // Agregar detalle de venta
        Task<DetalleVenta> AddAsync(
            DetalleVenta detalle,
            CancellationToken cancellationToken = default);

        // Eliminar detalle de venta
        Task RemoveAsync(
            DetalleVenta detalle,
            CancellationToken cancellationToken = default);
    }
}