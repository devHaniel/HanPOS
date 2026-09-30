using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Producto.
    /// Define la lógica de negocio para la entidad Producto.
    /// </summary>
    public interface IProductoService
    {
        // Obtener todos los productos activos
        Task<IReadOnlyList<Producto>> GetActivosAsync(CancellationToken cancellationToken = default);

        // Crear producto
        Task<Producto> CrearAsync(Producto producto, CancellationToken cancellationToken = default);

        // Actualizar producto
        Task<bool> ActualizarAsync(Producto producto, CancellationToken cancellationToken = default);

        // Eliminar producto
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}