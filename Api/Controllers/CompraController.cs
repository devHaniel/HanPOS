using Api.DTOs.Compra;
using Api.DTOs.DetalleCompra;
using Api.DTOs.Paginacion;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class CompraController : ControllerBase
    {
        private readonly ICompraService _compraService;

        public CompraController(ICompraService compraService)
        {
            _compraService = compraService;
        }

        /// <summary>
        /// Gets all purchases with pagination. (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResult<CompraResponseDto>>> GetAll(
            int pagina = 1,
            int cantidad = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _compraService.GetAllPagedAsync(pagina, cantidad, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets a purchase by ID. (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
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
        /// Gets purchases by date. (Admin only)
        /// </summary>
        [HttpGet("por-fecha/{fecha}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<CompraResponseDto>>> GetPorFecha(
            DateTime fecha,
            CancellationToken cancellationToken = default)
        {
            var compras = await _compraService.GetPorFechaAsync(fecha, cancellationToken);
            return Ok(compras);
        }

        /// <summary>
        /// Gets purchases by provider. (Admin only)
        /// </summary>
        [HttpGet("por-proveedor/{proveedorId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<CompraResponseDto>>> GetPorProveedor(
            int proveedorId,
            CancellationToken cancellationToken = default)
        {
            var compras = await _compraService.GetPorProveedorAsync(proveedorId, cancellationToken);
            return Ok(compras);
        }

        /// <summary>
        /// Gets details of a purchase. (Admin only)
        /// </summary>
        [HttpGet("{id}/detalles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<DetalleCompraResponseDto>>> GetDetalles(
            int id,
            CancellationToken cancellationToken = default)
        {
            var detalles = await _compraService.GetDetallesAsync(id, cancellationToken);
            return Ok(detalles);
        }

        /// <summary>
        /// Gets purchases by cash register. (Admin only)
        /// </summary>
        [HttpGet("por-caja/{cajaId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<CompraResponseDto>>> GetPorCaja(
            int cajaId,
            CancellationToken cancellationToken = default)
        {
            var compras = await _compraService.GetPorCajaAsync(cajaId, cancellationToken);
            return Ok(compras);
        }

        /// <summary>
        /// Creates a new purchase. (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CompraResponseDto>> Crear(
            [FromBody] CompraCrearRequest dto,
            CancellationToken cancellationToken = default)
        {
            var compra = await _compraService.CrearAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = compra.Id }, compra);
        }

        /// <summary>
        /// Updates an existing purchase. (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
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
        /// Deletes a purchase. (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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