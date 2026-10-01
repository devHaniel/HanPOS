using Api.DTOs.MovimientoCaja;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for managing MovimientoCaja entities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientoCajaController : ControllerBase
    {
        private readonly IMovimientoCajaService _movimientoCajaService;

        public MovimientoCajaController(IMovimientoCajaService movimientoCajaService)
        {
            _movimientoCajaService = movimientoCajaService;
        }

        /// <summary>
        /// Gets all cash movements.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MovimientoCajaResponseDto>>> GetAll(
            CancellationToken cancellationToken = default)
        {
            var movimientos = await _movimientoCajaService.GetAllAsync(cancellationToken);
            return Ok(movimientos);
        }

        /// <summary>
        /// Gets a cash movement by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<MovimientoCajaResponseDto>> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var movimiento = await _movimientoCajaService.GetByIdAsync(id, cancellationToken);
            if (movimiento == null)
                return NotFound();

            return Ok(movimiento);
        }

        /// <summary>
        /// Gets cash movements by cash register.
        /// </summary>
        [HttpGet("por-caja/{cajaId}")]
        public async Task<ActionResult<IReadOnlyList<MovimientoCajaResponseDto>>> GetPorCaja(
            int cajaId,
            CancellationToken cancellationToken = default)
        {
            var movimientos = await _movimientoCajaService.GetPorCajaAsync(cajaId, cancellationToken);
            return Ok(movimientos);
        }

        /// <summary>
        /// Creates a new cash movement.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MovimientoCajaResponseDto>> Crear(
            [FromBody] MovimientoCajaCrearRequest dto,
            CancellationToken cancellationToken = default)
        {
            var movimiento = await _movimientoCajaService.CrearAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = movimiento.Id }, movimiento);
        }

        /// <summary>
        /// Updates an existing cash movement.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<MovimientoCajaResponseDto>> Actualizar(
            int id,
            [FromBody] MovimientoCajaActualizarRequest dto,
            CancellationToken cancellationToken = default)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var movimiento = await _movimientoCajaService.ActualizarAsync(dto, cancellationToken);
            if (movimiento == null)
                return NotFound();

            return Ok(movimiento);
        }

        /// <summary>
        /// Deletes a cash movement.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(
            int id,
            CancellationToken cancellationToken = default)
        {
            var success = await _movimientoCajaService.EliminarAsync(id, cancellationToken);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}