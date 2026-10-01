using Api.DTOs.Categoria;
using Api.DTOs.Producto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Categoria.
    /// Define la lógica de negocio para la entidad Categoria.
    /// </summary>
    public interface ICategoriaService
    {
        // Obtener todas las categorías activas
        Task<IReadOnlyList<CategoriaResponseDto>> GetActivasAsync(CancellationToken cancellationToken = default);

        // Obtener categoría por ID
        Task<CategoriaResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Obtener productos de una categoría
        Task<IReadOnlyList<ProductoResponseDto>> GetProductosAsync(int categoriaId, CancellationToken cancellationToken = default);

        // Crear categoría
        Task<CategoriaResponseDto> CrearAsync(CategoriaCrearRequest dto, CancellationToken cancellationToken = default);

        // Actualizar categoría
        Task<CategoriaResponseDto?> ActualizarAsync(CategoriaActualizarRequest dto, CancellationToken cancellationToken = default);

        // Eliminar categoría (soft delete)
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}