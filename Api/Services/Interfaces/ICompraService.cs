using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Compra.
    /// Define la lógica de negocio para la entidad Compra.
    /// </summary>
    public interface ICompraService
    {
        // Obtener todas las compras
        Task<IReadOnlyList<Compra>> GetAllAsync(CancellationToken cancellationToken = default);

        // Obtener compra con detalles y proveedor
        Task<Compra?> GetWithDetailsAndProveedorAsync(int compraId, CancellationToken cancellationToken = default);

        // Obtener compras por fecha
        Task<IReadOnlyList<Compra>> GetPorFechaAsync(DateTime fecha, CancellationToken cancellationToken = default);

        // Obtener compras por proveedor
        Task<IReadOnlyList<Compra>> GetPorProveedorAsync(int proveedorId, CancellationToken cancellationToken = default);

        // Crear compra
        Task<Compra> CrearAsync(Compra compra, CancellationToken cancellationToken = default);

        // Actualizar compra
        Task<bool> ActualizarAsync(Compra compra, CancellationToken cancellationToken = default);

        // Eliminar compra
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}