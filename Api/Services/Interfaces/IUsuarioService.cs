using Api.DTOs.Usuario;
using Api.DTOs.Venta;
using Api.DTOs.Compra;
using Api.DTOs.Caja;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Usuario.
    /// Define la lógica de negocio para la entidad Usuario.
    /// </summary>
    public interface IUsuarioService
    {
        // Obtener todos los usuarios activos
        Task<IReadOnlyList<UsuarioResponseDto>> GetActivosAsync(CancellationToken cancellationToken = default);

        // Obtener usuario por ID
        Task<UsuarioResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Obtener usuario por Username
        Task<UsuarioResponseDto?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

        // Obtener ventas de un usuario
        Task<IReadOnlyList<VentaResponseDto>> GetVentasAsync(int usuarioId, CancellationToken cancellationToken = default);

        // Obtener compras de un usuario
        Task<IReadOnlyList<CompraResponseDto>> GetComprasAsync(int usuarioId, CancellationToken cancellationToken = default);

        // Obtener cajas de un usuario
        Task<IReadOnlyList<CajaResponseDto>> GetCajasAsync(int usuarioId, CancellationToken cancellationToken = default);

        // Crear usuario (valida username único)
        Task<UsuarioResponseDto> CrearAsync(UsuarioCrearRequest dto, CancellationToken cancellationToken = default);

        // Actualizar usuario
        Task<UsuarioResponseDto?> ActualizarAsync(UsuarioActualizarRequest dto, CancellationToken cancellationToken = default);

        // Eliminar usuario (soft delete)
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}