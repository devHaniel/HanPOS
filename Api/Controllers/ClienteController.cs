using Api.DTOs.Cliente;
using Api.DTOs.Venta;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for managing Cliente entities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        /// <summary>
        /// Gets all clients. (Admin, Vendedor)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<ActionResult<IReadOnlyList<ClienteResponseDto>>> GetAll(
            CancellationToken cancellationToken = default)
        {
            var clientes = await _clienteService.GetAllAsync(cancellationToken);
            return Ok(clientes);
        }

        /// <summary>
        /// Gets a client by ID. (Admin, Vendedor)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<ActionResult<ClienteResponseDto>> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var cliente = await _clienteService.GetByIdAsync(id, cancellationToken);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        /// <summary>
        /// Gets a client by RTN. (Admin, Vendedor)
        /// </summary>
        [HttpGet("rtn/{rtn}")]
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<ActionResult<ClienteResponseDto>> GetByRtn(
            string rtn,
            CancellationToken cancellationToken = default)
        {
            var cliente = await _clienteService.GetByRtnAsync(rtn, cancellationToken);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        /// <summary>
        /// Gets sales for a client. (Admin, Vendedor)
        /// </summary>
        [HttpGet("{id}/ventas")]
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<ActionResult<IReadOnlyList<VentaResponseDto>>> GetVentas(
            int id,
            CancellationToken cancellationToken = default)
        {
            var ventas = await _clienteService.GetVentasAsync(id, cancellationToken);
            return Ok(ventas);
        }

        /// <summary>
        /// Creates a new client. (Admin, Vendedor)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<ActionResult<ClienteResponseDto>> Crear(
            [FromBody] ClienteCrearRequest dto,
            CancellationToken cancellationToken = default)
        {
            var cliente = await _clienteService.CrearAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }

        /// <summary>
        /// Updates an existing client. (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClienteResponseDto>> Actualizar(
            int id,
            [FromBody] ClienteActualizarRequest dto,
            CancellationToken cancellationToken = default)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var cliente = await _clienteService.ActualizarAsync(dto, cancellationToken);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        /// <summary>
        /// Deletes a client. (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Eliminar(
            int id,
            CancellationToken cancellationToken = default)
        {
            var success = await _clienteService.EliminarAsync(id, cancellationToken);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}