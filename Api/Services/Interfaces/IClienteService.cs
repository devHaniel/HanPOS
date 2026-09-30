using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Cliente.
    /// Define la lógica de negocio para la entidad Cliente.
    /// </summary>
    public interface IClienteService
    {
        // Obtener todos los clientes
        Task<IReadOnlyList<Cliente>> GetAllAsync(CancellationToken cancellationToken = default);

        // Obtener con sus ventas
        Task<Cliente?> GetWithVentasAsync(CancellationToken cancellationToken = default);

        // Obtener por RTN
        Task<Cliente?> GetByRtnAsync(string rtn, CancellationToken cancellationToken = default);

        // Crear cliente
        Task<Cliente> CrearAsync(Cliente cliente, CancellationToken cancellationToken = default);

        // Actualizar cliente
        Task<bool> ActualizarAsync(Cliente cliente, CancellationToken cancellationToken = default);

        // Eliminar cliente
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}