using Api.DTOs.Proveedor;
using Api.DTOs.Compra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Proveedor.
    /// Define la lógica de negocio para la entidad Proveedor.
    /// </summary>
    public interface IProveedorService
    {
        // Obtener todos los proveedores
        Task<IReadOnlyList<ProveedorResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Obtener proveedor por ID
        Task<ProveedorResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Obtener por RTN
        Task<ProveedorResponseDto?> GetByRtnAsync(string rtn, CancellationToken cancellationToken = default);

        // Obtener compras de un proveedor
        Task<IReadOnlyList<CompraResponseDto>> GetComprasAsync(int proveedorId, CancellationToken cancellationToken = default);

        // Crear proveedor
        Task<ProveedorResponseDto> CrearAsync(ProveedorCrearRequest dto, CancellationToken cancellationToken = default);

        // Actualizar proveedor
        Task<ProveedorResponseDto?> ActualizarAsync(ProveedorActualizarRequest dto, CancellationToken cancellationToken = default);

        // Eliminar proveedor
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}