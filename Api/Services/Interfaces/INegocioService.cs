using Api.DTOs.Negocio;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de Negocio.
    /// Define la lógica de negocio para la entidad Negocio (singleton).
    /// </summary>
    public interface INegocioService
    {
        /// <summary>
        /// Obtiene la configuración del negocio (único registro).
        /// </summary>
        Task<NegocioResponseDto?> GetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Crea la configuración inicial del negocio.
        /// Solo se permite si no existe ninguno.
        /// </summary>
        Task<NegocioResponseDto> CrearAsync(NegocioCrearRequest dto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Actualiza la configuración del negocio.
        /// </summary>
        Task<NegocioResponseDto?> ActualizarAsync(NegocioActualizarRequest dto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si ya existe un negocio configurado.
        /// </summary>
        Task<bool> ExisteAsync(CancellationToken cancellationToken = default);
    }
}