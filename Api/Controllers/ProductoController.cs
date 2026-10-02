using Api.DTOs.Producto;
using Api.DTOs.DetalleVenta;
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
    /// Controller for managing Producto entities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        /// <summary>
        /// Gets all active products with pagination. (Admin, Vendedor)
        /// </summary>
        [HttpGet("activos")]
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<ActionResult<PagedResult<ProductoResponseDto>>> GetActivos(
            int pagina = 1,
            int cantidad = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _productoService.GetActivosPagedAsync(pagina, cantidad, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets a product by ID. (Admin, Vendedor)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<ActionResult<ProductoResponseDto>> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var producto = await _productoService.GetByIdAsync(id, cancellationToken);
            if (producto == null)
                return NotFound();

            return Ok(producto);
        }

        /// <summary>
        /// Gets a product by code. (Admin, Vendedor)
        /// </summary>
        [HttpGet("codigo/{codigo}")]
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<ActionResult<ProductoResponseDto>> GetByCodigo(
            string codigo,
            CancellationToken cancellationToken = default)
        {
            var producto = await _productoService.GetByCodigoAsync(codigo, cancellationToken);
            if (producto == null)
                return NotFound();

            return Ok(producto);
        }

        /// <summary>
        /// Gets sales (detalles) for a product. (Admin only)
        /// </summary>
        [HttpGet("{id}/ventas")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<DetalleVentaResponseDto>>> GetVentas(
            int id,
            CancellationToken cancellationToken = default)
        {
            var ventas = await _productoService.GetVentasAsync(id, cancellationToken);
            return Ok(ventas);
        }

        /// <summary>
        /// Gets purchases (detalles) for a product. (Admin only)
        /// </summary>
        [HttpGet("{id}/compras")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<DetalleCompraResponseDto>>> GetCompras(
            int id,
            CancellationToken cancellationToken = default)
        {
            var compras = await _productoService.GetComprasAsync(id, cancellationToken);
            return Ok(compras);
        }

        /// <summary>
        /// Creates a new product. (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductoResponseDto>> Crear(
            [FromBody] ProductoCrearRequest dto,
            CancellationToken cancellationToken = default)
        {
            var creado = await _productoService.CrearAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }

        /// <summary>
        /// Updates an existing product. (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductoResponseDto>> Actualizar(
            int id,
            [FromBody] ProductoActualizarRequest dto,
            CancellationToken cancellationToken = default)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var actualizado = await _productoService.ActualizarAsync(dto, cancellationToken);
            if (actualizado == null)
                return NotFound();

            return Ok(actualizado);
        }

        /// <summary>
        /// Deletes a product (soft delete). (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Eliminar(
            int id,
            CancellationToken cancellationToken = default)
        {
            var success = await _productoService.EliminarAsync(id, cancellationToken);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}