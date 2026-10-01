using Api.DTOs.Venta;
using Api.DTOs.DetalleVenta;
using Api.DTOs.Paginacion;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for managing Venta entities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentaController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        /// <summary>
        /// Gets all sales with pagination.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<VentaResponseDto>>> GetAll(
            int pagina = 1,
            int cantidad = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _ventaService.GetAllPagedAsync(pagina, cantidad, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets a sale by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<VentaResponseDto>> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var venta = await _ventaService.GetByIdAsync(id, cancellationToken);
            if (venta == null)
                return NotFound();

            return Ok(venta);
        }

        /// <summary>
        /// Gets sales by date.
        /// </summary>
        [HttpGet("por-fecha/{fecha}")]
        public async Task<ActionResult<IReadOnlyList<VentaResponseDto>>> GetPorFecha(
            DateTime fecha,
            CancellationToken cancellationToken = default)
        {
            var ventas = await _ventaService.GetPorFechaAsync(fecha, cancellationToken);
            return Ok(ventas);
        }

        /// <summary>
        /// Gets sales by user.
        /// </summary>
        [HttpGet("por-usuario/{usuarioId}")]
        public async Task<ActionResult<IReadOnlyList<VentaResponseDto>>> GetPorUsuario(
            int usuarioId,
            CancellationToken cancellationToken = default)
        {
            var ventas = await _ventaService.GetPorUsuarioAsync(usuarioId, cancellationToken);
            return Ok(ventas);
        }

        /// <summary>
        /// Gets details of a sale.
        /// </summary>
        [HttpGet("{id}/detalles")]
        public async Task<ActionResult<IReadOnlyList<DetalleVentaResponseDto>>> GetDetalles(
            int id,
            CancellationToken cancellationToken = default)
        {
            var detalles = await _ventaService.GetDetallesAsync(id, cancellationToken);
            return Ok(detalles);
        }

        /// <summary>
        /// Creates a new sale.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<VentaResponseDto>> Crear(
            [FromBody] VentaCrearRequest dto,
            CancellationToken cancellationToken = default)
        {
            var venta = await _ventaService.CrearAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = venta.Id }, venta);
        }

        /// <summary>
        /// Updates an existing sale.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<VentaResponseDto>> Actualizar(
            int id,
            [FromBody] VentaActualizarRequest dto,
            CancellationToken cancellationToken = default)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var venta = await _ventaService.ActualizarAsync(dto, cancellationToken);
            if (venta == null)
                return NotFound();

            return Ok(venta);
        }

        /// <summary>
        /// Deletes a sale.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(
            int id,
            CancellationToken cancellationToken = default)
        {
            var success = await _ventaService.EliminarAsync(id, cancellationToken);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}