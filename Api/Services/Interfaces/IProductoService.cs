using Api.DTOs.Producto;
using Api.DTOs.Paginacion;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Producto.
    /// Define la lógica de negocio para la entidad Producto.
    /// </summary>
    public interface IProductoService
    {
        // Obtener todos los productos activos
        Task<IReadOnlyList<ProductoResponseDto>> GetActivosAsync(CancellationToken cancellationToken = default);

        // Obtener todos los productos activos con paginación
        Task<PagedResult<ProductoResponseDto>> GetActivosPagedAsync(int pagina = 1, int cantidad = 10, CancellationToken cancellationToken = default);

        // Buscar productos por nombre o código con paginación
        Task<PagedResult<ProductoResponseDto>> BuscarAsync(string? termino, int pagina = 1, int cantidad = 10, CancellationToken cancellationToken = default);

        // Obtener producto por ID
        Task<ProductoResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Obtener producto por código
        Task<ProductoResponseDto?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);

        // Obtener ventas de un producto
        Task<IReadOnlyList<Api.DTOs.DetalleVenta.DetalleVentaResponseDto>> GetVentasAsync(int productoId, CancellationToken cancellationToken = default);

        // Obtener compras de un producto
        Task<IReadOnlyList<Api.DTOs.DetalleCompra.DetalleCompraResponseDto>> GetComprasAsync(int productoId, CancellationToken cancellationToken = default);

        // Crear producto
        Task<ProductoResponseDto> CrearAsync(ProductoCrearRequest dto, CancellationToken cancellationToken = default);

        // Actualizar producto
        Task<ProductoResponseDto?> ActualizarAsync(ProductoActualizarRequest dto, CancellationToken cancellationToken = default);

        // Eliminar producto (soft delete)
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}