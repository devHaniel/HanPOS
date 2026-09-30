using Api.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Caja.
    /// Define la lógica de negocio para la entidad Caja.
    /// </summary>
    public interface ICajaService
    {
        // Obtener todas las cajas
        Task<IReadOnlyList<Caja>> GetAllAsync(CancellationToken cancellationToken = default);

        // Obtener caja abierta actual
        Task<Caja?> GetAbiertaAsync(CancellationToken cancellationToken = default);

        // Crear nueva caja
        Task<Caja> CrearAsync(Caja caja, CancellationToken cancellationToken = default);

        // Cerrar caja
        Task<Caja?> CerrarAsync(int id, decimal montoFinal, CancellationToken cancellationToken = default);

        // Eliminar caja
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}