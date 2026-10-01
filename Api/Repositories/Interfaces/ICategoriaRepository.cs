using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de Categoria.
    /// Define el contrato para operaciones sobre la entidad Categoria.
    /// </summary>
    public interface ICategoriaRepository
    {
        // Obtener todas las categorías activas
        Task<IReadOnlyList<Categoria>> GetActivasAsync(
            CancellationToken cancellationToken = default);

        // Obtener categoría por ID
        Task<Categoria?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        // Obtener con sus productos
        Task<Categoria?> GetWithProductosAsync(
            CancellationToken cancellationToken = default);

        // Agregar categoría
        Task<Categoria> AddAsync(
            Categoria categoria,
            CancellationToken cancellationToken = default);

        // Eliminar categoría
        Task RemoveAsync(
            Categoria categoria,
            CancellationToken cancellationToken = default);
    }
}