using Api.DTOs.Compra;
using Api.DTOs.DetalleCompra;
using Api.DTOs.Paginacion;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Compra.
    /// Define la lógica de negocio para la entidad Compra.
    /// </summary>
    public interface ICompraService
    {
        // Obtener todas las compras
        Task<IReadOnlyList<CompraResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Obtener todas las compras con paginación
        Task<PagedResult<CompraResponseDto>> GetAllPagedAsync(int pagina = 1, int cantidad = 10, CancellationToken cancellationToken = default);

        // Obtener compra por ID
        Task<CompraResponseDto?> GetByIdAsync(int compraId, CancellationToken cancellationToken = default);

        // Obtener compras por fecha
        Task<IReadOnlyList<CompraResponseDto>> GetPorFechaAsync(DateTime fecha, CancellationToken cancellationToken = default);

        // Obtener compras por proveedor
        Task<IReadOnlyList<CompraResponseDto>> GetPorProveedorAsync(int proveedorId, CancellationToken cancellationToken = default);

        // Obtener detalles de una compra
        Task<IReadOnlyList<DetalleCompraResponseDto>> GetDetallesAsync(int compraId, CancellationToken cancellationToken = default);

        // Crear compra
        Task<CompraResponseDto> CrearAsync(CompraCrearRequest dto, CancellationToken cancellationToken = default);

        // Actualizar compra
        Task<CompraResponseDto?> ActualizarAsync(CompraActualizarRequest dto, CancellationToken cancellationToken = default);

        // Eliminar compra
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}