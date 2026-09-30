using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Venta.
    /// Define la lógica de negocio para la entidad Venta.
    /// </summary>
    public interface IVentaService
    {
        // Obtener todas las ventas
        Task<IReadOnlyList<Venta>> GetAllAsync(CancellationToken cancellationToken = default);

        // Obtener venta con detalles y cliente
        Task<Venta?> GetWithDetailsAndClienteAsync(int ventaId, CancellationToken cancellationToken = default);

        // Obtener ventas por fecha
        Task<IReadOnlyList<Venta>> GetPorFechaAsync(DateTime fecha, CancellationToken cancellationToken = default);

        // Obtener ventas por usuario
        Task<IReadOnlyList<Venta>> GetPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default);

        // Crear venta
        Task<Venta> CrearAsync(Venta venta, CancellationToken cancellationToken = default);

        // Actualizar venta
        Task<bool> ActualizarAsync(Venta venta, CancellationToken cancellationToken = default);

        // Eliminar venta
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}