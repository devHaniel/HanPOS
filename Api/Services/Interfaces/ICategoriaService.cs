using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Categoria.
    /// Define la lógica de negocio para la entidad Categoria.
    /// </summary>
    public interface ICategoriaService
    {
        // Obtener todas las categorías activas
        Task<IReadOnlyList<Categoria>> GetActivasAsync(CancellationToken cancellationToken = default);

        // Crear categoría
        Task<Categoria> CrearAsync(Categoria categoria, CancellationToken cancellationToken = default);

        // Actualizar categoría
        Task<bool> ActualizarAsync(Categoria categoria, CancellationToken cancellationToken = default);

        // Eliminar categoría
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}