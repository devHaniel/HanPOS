using Api.DTOs.Cliente;
using Api.DTOs.Venta;
using Api.Services.Interfaces;
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
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        /// <summary>
        /// Gets all clients.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ClienteResponseDto>>> GetAll(
            CancellationToken cancellationToken = default)
        {
            var clientes = await _clienteService.GetAllAsync(cancellationToken);
            return Ok(clientes);
        }

        /// <summary>
        /// Gets a client by ID.
        /// </summary>
        [HttpGet("{id}")]
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
        /// Gets a client by RTN.
        /// </summary>
        [HttpGet("rtn/{rtn}")]
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
        /// Gets sales for a client.
        /// </summary>
        [HttpGet("{id}/ventas")]
        public async Task<ActionResult<IReadOnlyList<VentaResponseDto>>> GetVentas(
            int id,
            CancellationToken cancellationToken = default)
        {
            var ventas = await _clienteService.GetVentasAsync(id, cancellationToken);
            return Ok(ventas);
        }

        /// <summary>
        /// Creates a new client.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ClienteResponseDto>> Crear(
            [FromBody] ClienteCrearRequest dto,
            CancellationToken cancellationToken = default)
        {
            var cliente = await _clienteService.CrearAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }

        /// <summary>
        /// Updates an existing client.
        /// </summary>
        [HttpPut("{id}")]
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
        /// Deletes a client.
        /// </summary>
        [HttpDelete("{id}")]
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