using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de Proveedor.
    /// Define el contrato para operaciones sobre la entidad Proveedor.
    /// </summary>
    public interface IProveedorRepository
    {
        // Obtener todos los proveedores
        Task<IReadOnlyList<Proveedor>> GetAllAsync(
            CancellationToken cancellationToken = default);

        // Obtener proveedor por ID
        Task<Proveedor?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        // Obtener con sus compras
        Task<Proveedor?> GetWithComprasAsync(
            CancellationToken cancellationToken = default);

        // Obtener por RTN
        Task<Proveedor?> GetByRtnAsync(
            string rtn,
            CancellationToken cancellationToken = default);

        // Agregar proveedor
        Task<Proveedor> AddAsync(
            Proveedor proveedor,
            CancellationToken cancellationToken = default);

        // Eliminar proveedor
        Task RemoveAsync(
            Proveedor proveedor,
            CancellationToken cancellationToken = default);
    }
}