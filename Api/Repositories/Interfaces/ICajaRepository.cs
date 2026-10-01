using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de Caja.
    /// Define el contrato para operaciones sobre la entidad Caja.
    /// </summary>
    public interface ICajaRepository
    {
        // Obtener todas las cajas
        Task<IReadOnlyList<Caja>> GetAllAsync(
            CancellationToken cancellationToken = default);

        // Obtener todas las cajas con paginación
        Task<(List<Caja> Items, int Total)> GetAllPagedAsync(
            int pagina,
            int cantidad,
            CancellationToken cancellationToken = default);

        // Obtener caja por ID
        Task<Caja?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        // Obtener caja abierta actual
        Task<Caja?> GetAbiertaAsync(
            CancellationToken cancellationToken = default);

        // Agregar nueva caja
        Task<Caja> AddAsync(
            Caja caja,
            CancellationToken cancellationToken = default);

        // Eliminar caja
        Task RemoveAsync(
            Caja caja,
            CancellationToken cancellationToken = default);
    }
}