using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Api.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de DetalleCompra.
    /// Define el contrato para operaciones sobre la entidad DetalleCompra.
    /// </summary>
    public interface IDetalleCompraRepository
    {
        // Obtener todos los detalles de compra
        Task<IReadOnlyList<DetalleCompra>> GetAllAsync(
            CancellationToken cancellationToken = default);

        // Obtener por compra
        Task<IReadOnlyList<DetalleCompra>> GetPorCompraAsync(
            int compraId,
            CancellationToken cancellationToken = default);

        // Agregar detalle de compra
        Task<DetalleCompra> AddAsync(
            DetalleCompra detalle,
            CancellationToken cancellationToken = default);

        // Eliminar detalle de compra
        Task RemoveAsync(
            DetalleCompra detalle,
            CancellationToken cancellationToken = default);
    }
}