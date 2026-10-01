using Api.DTOs.Caja;
using Api.DTOs.Paginacion;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Caja.
    /// Define la lógica de negocio para la entidad Caja.
    /// </summary>
    public interface ICajaService
    {
        // Obtener todas las cajas
        Task<IReadOnlyList<CajaResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Obtener todas las cajas con paginación
        Task<PagedResult<CajaResponseDto>> GetAllPagedAsync(int pagina = 1, int cantidad = 10, CancellationToken cancellationToken = default);

        // Obtener caja por ID
        Task<CajaResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Obtener caja abierta actual
        Task<CajaResponseDto?> GetAbiertaAsync(CancellationToken cancellationToken = default);

        // Crear nueva caja
        Task<CajaResponseDto> CrearAsync(CajaCrearRequest dto, CancellationToken cancellationToken = default);

        // Cerrar caja
        Task<CajaResponseDto?> CerrarAsync(int id, CancellationToken cancellationToken = default);

        // Actualizar caja
        Task<CajaResponseDto?> ActualizarAsync(CajaActualizarRequest dto, CancellationToken cancellationToken = default);

        // Eliminar caja
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);

        // Obtener movimientos de una caja
        Task<IReadOnlyList<Api.DTOs.MovimientoCaja.MovimientoCajaResponseDto>> GetMovimientosAsync(int cajaId, CancellationToken cancellationToken = default);
    }
}