using Api.Models.Entities;
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
        Task<IReadOnlyList<Usuario>> GetActivosAsync(CancellationToken cancellationToken = default);

        // Crear usuario (valida username único)
        Task<Usuario> CrearAsync(Usuario usuario, CancellationToken cancellationToken = default);

        // Actualizar usuario
        Task<bool> ActualizarAsync(Usuario usuario, CancellationToken cancellationToken = default);

        // Eliminar usuario
        Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    }
}