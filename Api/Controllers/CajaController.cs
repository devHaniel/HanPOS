using Api.DTOs.Caja;
using Api.DTOs.MovimientoCaja;
using Api.DTOs.Paginacion;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for managing Caja entities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CajaController : ControllerBase
    {
        private readonly ICajaService _cajaService;

        public CajaController(ICajaService cajaService)
        {
            _cajaService = cajaService;
        }

        /// <summary>
        /// Gets all cash registers with pagination.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<CajaResponseDto>>> GetAll(
            int pagina = 1,
            int cantidad = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _cajaService.GetAllPagedAsync(pagina, cantidad, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets a cash register by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CajaResponseDto>> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var caja = await _cajaService.GetByIdAsync(id, cancellationToken);
            if (caja == null)
                return NotFound();

            return Ok(caja);
        }

        /// <summary>
        /// Gets the currently open cash register.
        /// </summary>
        [HttpGet("abierta")]
        public async Task<ActionResult<CajaResponseDto>> GetAbierta(
            CancellationToken cancellationToken = default)
        {
            var caja = await _cajaService.GetAbiertaAsync(cancellationToken);
            if (caja == null)
                return NotFound();

            return Ok(caja);
        }

        /// <summary>
        /// Creates a new cash register (opens a new cash session).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CajaResponseDto>> Crear(
            [FromBody] CajaCrearRequest dto,
            CancellationToken cancellationToken = default)
        {
            var caja = await _cajaService.CrearAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = caja.Id }, caja);
        }

        /// <summary>
        /// Closes a cash register.
        /// </summary>
        [HttpPost("{id}/cerrar")]
        public async Task<ActionResult<CajaResponseDto>> Cerrar(
            int id,
            CancellationToken cancellationToken = default)
        {
            var caja = await _cajaService.CerrarAsync(id, cancellationToken);
            if (caja == null)
                return NotFound();

            return Ok(caja);
        }

        /// <summary>
        /// Updates a cash register.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<CajaResponseDto>> Actualizar(
            int id,
            [FromBody] CajaActualizarRequest dto,
            CancellationToken cancellationToken = default)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var caja = await _cajaService.ActualizarAsync(dto, cancellationToken);
            if (caja == null)
                return NotFound();

            return Ok(caja);
        }

        /// <summary>
        /// Deletes a cash register.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(
            int id,
            CancellationToken cancellationToken = default)
        {
            var success = await _cajaService.EliminarAsync(id, cancellationToken);
            if (!success)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Gets movements for a cash register.
        /// </summary>
        [HttpGet("{cajaId}/movimientos")]
        public async Task<ActionResult<IReadOnlyList<MovimientoCajaResponseDto>>> GetMovimientos(
            int cajaId,
            CancellationToken cancellationToken = default)
        {
            var movimientos = await _cajaService.GetMovimientosAsync(cajaId, cancellationToken);
            return Ok(movimientos);
        }
    }
}