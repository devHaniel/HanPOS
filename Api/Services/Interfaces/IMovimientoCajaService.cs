using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Entities;

namespace Api.Services.Interfaces
{
    public interface IMovimientoCajaService
    {
        // Obtener todos los movimientos
        Task<IReadOnlyList<MovimientoCaja>> GetAllAsync(
            CancellationToken cancellationToken = default);

        // Obtener por caja
        Task<IReadOnlyList<MovimientoCaja>> GetPorCajaAsync(
            int cajaId,
            CancellationToken cancellationToken = default);

        // Obtener por id
        Task<MovimientoCaja?> GetPorId(int id);

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