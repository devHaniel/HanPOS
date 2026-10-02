using Api.DTOs.Usuario;
using Api.DTOs.Venta;
using Api.DTOs.Compra;
using Api.DTOs.Caja;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for managing Usuario entities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Gets all active users. (Admin only)
        /// </summary>
        [HttpGet("activos")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<UsuarioResponseDto>>> GetActivos(
            CancellationToken cancellationToken = default)
        {
            var usuarios = await _usuarioService.GetActivosAsync(cancellationToken);
            return Ok(usuarios);
        }

        /// <summary>
        /// Gets a user by ID. (Admin or own profile)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioService.GetByIdAsync(id, cancellationToken);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        /// <summary>
        /// Gets a user by username. (Admin only)
        /// </summary>
        [HttpGet("username/{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UsuarioResponseDto>> GetByUsername(
            string username,
            CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioService.GetByUsernameAsync(username, cancellationToken);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        /// <summary>
        /// Gets sales for a user. (Admin or Vendedor - own sales)
        /// </summary>
        [HttpGet("{id}/ventas")]
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<ActionResult<IReadOnlyList<VentaResponseDto>>> GetVentas(
            int id,
            CancellationToken cancellationToken = default)
        {
            var ventas = await _usuarioService.GetVentasAsync(id, cancellationToken);
            return Ok(ventas);
        }

        /// <summary>
        /// Gets purchases for a user. (Admin only)
        /// </summary>
        [HttpGet("{id}/compras")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<CompraResponseDto>>> GetCompras(
            int id,
            CancellationToken cancellationToken = default)
        {
            var compras = await _usuarioService.GetComprasAsync(id, cancellationToken);
            return Ok(compras);
        }

        /// <summary>
        /// Gets cash registers for a user. (Admin or Vendedor - own cajas)
        /// </summary>
        [HttpGet("{id}/cajas")]
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<ActionResult<IReadOnlyList<CajaResponseDto>>> GetCajas(
            int id,
            CancellationToken cancellationToken = default)
        {
            var cajas = await _usuarioService.GetCajasAsync(id, cancellationToken);
            return Ok(cajas);
        }

        /// <summary>
        /// Creates a new user. (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UsuarioResponseDto>> Crear(
            [FromBody] UsuarioCrearRequest dto,
            CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioService.CrearAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }

        /// <summary>
        /// Updates an existing user. (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UsuarioResponseDto>> Actualizar(
            int id,
            [FromBody] UsuarioActualizarRequest dto,
            CancellationToken cancellationToken = default)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var usuario = await _usuarioService.ActualizarAsync(dto, cancellationToken);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        /// <summary>
        /// Deletes a user (soft delete). (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Eliminar(
            int id,
            CancellationToken cancellationToken = default)
        {
            var success = await _usuarioService.EliminarAsync(id, cancellationToken);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}