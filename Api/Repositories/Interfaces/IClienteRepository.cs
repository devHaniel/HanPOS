using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de Cliente.
    /// Define el contrato para operaciones sobre la entidad Cliente.
    /// </summary>
    public interface IClienteRepository
    {
        // Obtener todos los clientes
        Task<IReadOnlyList<Cliente>> GetAllAsync(
            CancellationToken cancellationToken = default);

        // Obtener con sus ventas
        Task<Cliente?> GetWithVentasAsync(
            CancellationToken cancellationToken = default);

        // Obtener por RTN
        Task<Cliente?> GetByRtnAsync(
            string rtn,
            CancellationToken cancellationToken = default);

        // Agregar cliente
        Task<Cliente> AddAsync(
            Cliente cliente,
            CancellationToken cancellationToken = default);

        // Eliminar cliente
        Task RemoveAsync(
            Cliente cliente,
            CancellationToken cancellationToken = default);
    }
}