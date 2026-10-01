using Api.DTOs.Cliente;
using Api.DTOs.Venta;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Cliente.
    /// Define la lógica de negocio para la entidad Cliente.
    /// </summary>
    public interface IClienteService
    {
        // Obtener todos los clientes
        Task<IReadOnlyList<ClienteResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Obtener cliente por ID
        Task<ClienteResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Obtener por RTN
        Task<ClienteResponseDto?> GetByRtnAsync(string rtn, CancellationToken cancellationToken = default);

        // Obtener ventas de un cliente
        Task<IReadOnlyList<VentaResponseDto>> GetVentasAsync(int clienteId, CancellationToken cancellationToken = default);

        // Crear cliente
        Task<ClienteResponseDto> CrearAsync(ClienteCrearRequest dto, CancellationToken cancellationToken = default);

        // Actualizar cliente
        Task<ClienteResponseDto?> ActualizarAsync(ClienteActualizarRequest dto, CancellationToken cancellationToken = default);

        // Eliminar cliente
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}