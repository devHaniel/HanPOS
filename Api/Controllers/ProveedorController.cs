using Api.DTOs.Proveedor;
using Api.DTOs.Compra;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for managing Proveedor entities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProveedorController : ControllerBase
    {
        private readonly IProveedorService _proveedorService;

        public ProveedorController(IProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }

        /// <summary>
        /// Gets all providers. (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<ProveedorResponseDto>>> GetAll(
            CancellationToken cancellationToken = default)
        {
            var proveedores = await _proveedorService.GetAllAsync(cancellationToken);
            return Ok(proveedores);
        }

        /// <summary>
        /// Gets a provider by ID. (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProveedorResponseDto>> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var proveedor = await _proveedorService.GetByIdAsync(id, cancellationToken);
            if (proveedor == null)
                return NotFound();

            return Ok(proveedor);
        }

        /// <summary>
        /// Gets a provider by RTN. (Admin only)
        /// </summary>
        [HttpGet("rtn/{rtn}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProveedorResponseDto>> GetByRtn(
            string rtn,
            CancellationToken cancellationToken = default)
        {
            var proveedor = await _proveedorService.GetByRtnAsync(rtn, cancellationToken);
            if (proveedor == null)
                return NotFound();

            return Ok(proveedor);
        }

        /// <summary>
        /// Gets purchases for a provider. (Admin only)
        /// </summary>
        [HttpGet("{id}/compras")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<CompraResponseDto>>> GetCompras(
            int id,
            CancellationToken cancellationToken = default)
        {
            var compras = await _proveedorService.GetComprasAsync(id, cancellationToken);
            return Ok(compras);
        }

        /// <summary>
        /// Creates a new provider. (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProveedorResponseDto>> Crear(
            [FromBody] ProveedorCrearRequest dto,
            CancellationToken cancellationToken = default)
        {
            var proveedor = await _proveedorService.CrearAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = proveedor.Id }, proveedor);
        }

        /// <summary>
        /// Updates an existing provider. (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProveedorResponseDto>> Actualizar(
            int id,
            [FromBody] ProveedorActualizarRequest dto,
            CancellationToken cancellationToken = default)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var proveedor = await _proveedorService.ActualizarAsync(dto, cancellationToken);
            if (proveedor == null)
                return NotFound();

            return Ok(proveedor);
        }

        /// <summary>
        /// Deletes a provider. (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Eliminar(
            int id,
            CancellationToken cancellationToken = default)
        {
            var success = await _proveedorService.EliminarAsync(id, cancellationToken);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}