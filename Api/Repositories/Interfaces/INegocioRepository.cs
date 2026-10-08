using Api.Models.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de Negocio.
    /// Define el contrato para operaciones sobre la entidad Negocio (singleton).
    /// </summary>
    public interface INegocioRepository
    {
        /// <summary>
        /// Obtiene el negocio (único registro). Retorna null si no existe.
        /// </summary>
        Task<Negocio?> GetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene el negocio por ID.
        /// </summary>
        Task<Negocio?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Agrega el negocio (solo si no existe ninguno).
        /// </summary>
        Task<Negocio> AddAsync(Negocio negocio, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si ya existe un negocio registrado.
        /// </summary>
        Task<bool> ExisteAsync(CancellationToken cancellationToken = default);
    }
}