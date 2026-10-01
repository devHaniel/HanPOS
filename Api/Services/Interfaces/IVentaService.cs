using Api.DTOs.Venta;
using Api.DTOs.DetalleVenta;
using Api.DTOs.Paginacion;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Venta.
    /// Define la lógica de negocio para la entidad Venta.
    /// </summary>
    public interface IVentaService
    {
        // Obtener todas las ventas con paginación
        Task<PagedResult<VentaResponseDto>> GetAllPagedAsync(int pagina = 1, int cantidad = 10, CancellationToken cancellationToken = default);

        // Obtener todas las ventas (sin paginación - legacy)
        Task<IReadOnlyList<VentaResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Obtener venta por ID
        Task<VentaResponseDto?> GetByIdAsync(int ventaId, CancellationToken cancellationToken = default);

        // Obtener ventas por fecha
        Task<IReadOnlyList<VentaResponseDto>> GetPorFechaAsync(DateTime fecha, CancellationToken cancellationToken = default);

        // Obtener ventas por usuario
        Task<IReadOnlyList<VentaResponseDto>> GetPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default);

        // Obtener detalles de una venta
        Task<IReadOnlyList<DetalleVentaResponseDto>> GetDetallesAsync(int ventaId, CancellationToken cancellationToken = default);

        // Crear venta
        Task<VentaResponseDto> CrearAsync(VentaCrearRequest dto, CancellationToken cancellationToken = default);

        // Actualizar venta
        Task<VentaResponseDto?> ActualizarAsync(VentaActualizarRequest dto, CancellationToken cancellationToken = default);

        // Eliminar venta
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}