using Api.DTOs.Negocio;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Controller para gestionar la configuración del negocio (singleton).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NegocioController : ControllerBase
    {
        private readonly INegocioService _negocioService;

        public NegocioController(INegocioService negocioService)
        {
            _negocioService = negocioService;
        }

        /// <summary>
        /// Obtiene la configuración del negocio.
        /// Accesible para Admin y Vendedor (necesario para comprobantes, UI, etc.)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<ActionResult<NegocioResponseDto>> Get(CancellationToken cancellationToken = default)
        {
            var negocio = await _negocioService.GetAsync(cancellationToken);
            if (negocio == null)
                return NotFound("No se ha configurado el negocio aún.");

            return Ok(negocio);
        }

        /// <summary>
        /// Crea la configuración inicial del negocio.
        /// Solo Admin. Solo se permite una vez.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NegocioResponseDto>> Crear(
            [FromBody] NegocioCrearRequest dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var negocio = await _negocioService.CrearAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(Get), new { id = negocio.Id }, negocio);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza la configuración del negocio.
        /// Solo Admin.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NegocioResponseDto>> Actualizar(
            int id,
            [FromBody] NegocioActualizarRequest dto,
            CancellationToken cancellationToken = default)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var negocio = await _negocioService.ActualizarAsync(dto, cancellationToken);
            if (negocio == null)
                return NotFound();

            return Ok(negocio);
        }

        /// <summary>
        /// Verifica si ya existe un negocio configurado.
        /// Útil para el frontend para decidir si mostrar formulario de creación o edición.
        /// </summary>
        [HttpGet("existe")]
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<ActionResult<bool>> Existe(CancellationToken cancellationToken = default)
        {
            var existe = await _negocioService.ExisteAsync(cancellationToken);
            return Ok(existe);
        }
    }
}