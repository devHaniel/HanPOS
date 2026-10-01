using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de MovimientoCaja.
    /// Define el contrato para operaciones sobre la entidad MovimientoCaja.
    /// </summary>
    public interface IMovimientoCajaRepository
    {
        // Obtener todos los movimientos
        Task<IReadOnlyList<MovimientoCaja>> GetAllAsync(
            CancellationToken cancellationToken = default);

        // Obtener movimiento por ID
        Task<MovimientoCaja?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        // Obtener por caja
        Task<IReadOnlyList<MovimientoCaja>> GetPorCajaAsync(
            int cajaId,
            CancellationToken cancellationToken = default);

        // Agregar movimiento
        Task<MovimientoCaja> AddAsync(
            MovimientoCaja movimiento,
            CancellationToken cancellationToken = default);

        // Eliminar movimiento
        Task RemoveAsync(
            MovimientoCaja movimiento,
            CancellationToken cancellationToken = default);
    }
}