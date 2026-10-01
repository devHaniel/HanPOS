using Api.DTOs.Compra;
using Api.DTOs.DetalleCompra;
using Api.DTOs.Paginacion;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for managing Compra entities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CompraController : ControllerBase
    {
        private readonly ICompraService _compraService;

        public CompraController(ICompraService compraService)
        {
            _compraService = compraService;
        }

        /// <summary>
        /// Gets all purchases with pagination.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<CompraResponseDto>>> GetAll(
            int pagina = 1,
            int cantidad = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _compraService.GetAllPagedAsync(pagina, cantidad, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets a purchase by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CompraResponseDto>> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var compra = await _compraService.GetByIdAsync(id, cancellationToken);
            if (compra == null)
                return NotFound();

            return Ok(compra);
        }

        /// <summary>
        /// Gets purchases by date.
        /// </summary>
        [HttpGet("por-fecha/{fecha}")]
        public async Task<ActionResult<IReadOnlyList<CompraResponseDto>>> GetPorFecha(
            DateTime fecha,
            CancellationToken cancellationToken = default)
        {
            var compras = await _compraService.GetPorFechaAsync(fecha, cancellationToken);
            return Ok(compras);
        }

        /// <summary>
        /// Gets purchases by provider.
        /// </summary>
        [HttpGet("por-proveedor/{proveedorId}")]
        public async Task<ActionResult<IReadOnlyList<CompraResponseDto>>> GetPorProveedor(
            int proveedorId,
            CancellationToken cancellationToken = default)
        {
            var compras = await _compraService.GetPorProveedorAsync(proveedorId, cancellationToken);
            return Ok(compras);
        }

        /// <summary>
        /// Gets details of a purchase.
        /// </summary>
        [HttpGet("{id}/detalles")]
        public async Task<ActionResult<IReadOnlyList<DetalleCompraResponseDto>>> GetDetalles(
            int id,
            CancellationToken cancellationToken = default)
        {
            var detalles = await _compraService.GetDetallesAsync(id, cancellationToken);
            return Ok(detalles);
        }

        /// <summary>
        /// Creates a new purchase.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CompraResponseDto>> Crear(
            [FromBody] CompraCrearRequest dto,
            CancellationToken cancellationToken = default)
        {
            var compra = await _compraService.CrearAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = compra.Id }, compra);
        }

        /// <summary>
        /// Updates an existing purchase.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<CompraResponseDto>> Actualizar(
            int id,
            [FromBody] CompraActualizarRequest dto,
            CancellationToken cancellationToken = default)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var compra = await _compraService.ActualizarAsync(dto, cancellationToken);
            if (compra == null)
                return NotFound();

            return Ok(compra);
        }

        /// <summary>
        /// Deletes a purchase.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(
            int id,
            CancellationToken cancellationToken = default)
        {
            var success = await _compraService.EliminarAsync(id, cancellationToken);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}