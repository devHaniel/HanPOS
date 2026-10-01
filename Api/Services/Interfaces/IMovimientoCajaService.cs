using Api.DTOs.MovimientoCaja;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    public interface IMovimientoCajaService
    {
        // Obtener todos los movimientos
        Task<IReadOnlyList<MovimientoCajaResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Obtener movimiento por ID
        Task<MovimientoCajaResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Obtener por caja
        Task<IReadOnlyList<MovimientoCajaResponseDto>> GetPorCajaAsync(int cajaId, CancellationToken cancellationToken = default);

        // Agregar movimiento
        Task<MovimientoCajaResponseDto> CrearAsync(MovimientoCajaCrearRequest dto, CancellationToken cancellationToken = default);

        // Actualizar movimiento
        Task<MovimientoCajaResponseDto?> ActualizarAsync(MovimientoCajaActualizarRequest dto, CancellationToken cancellationToken = default);

        // Eliminar movimiento
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}