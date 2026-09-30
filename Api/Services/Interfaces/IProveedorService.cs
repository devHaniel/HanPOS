using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Proveedor.
    /// Define la lógica de negocio para la entidad Proveedor.
    /// </summary>
    public interface IProveedorService
    {
        // Obtener todos los proveedores
        Task<IReadOnlyList<Proveedor>> GetAllAsync(CancellationToken cancellationToken = default);

        // Obtener con sus compras
        Task<Proveedor?> GetWithComprasAsync(CancellationToken cancellationToken = default);

        // Obtener por RTN
        Task<Proveedor?> GetByRtnAsync(string rtn, CancellationToken cancellationToken = default);

        // Crear proveedor
        Task<Proveedor> CrearAsync(Proveedor proveedor, CancellationToken cancellationToken = default);

        // Actualizar proveedor
        Task<bool> ActualizarAsync(Proveedor proveedor, CancellationToken cancellationToken = default);

        // Eliminar proveedor
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}