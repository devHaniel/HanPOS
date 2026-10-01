using Api.DTOs.Categoria;
using Api.DTOs.Producto;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for managing Categoria entities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        /// <summary>
        /// Gets all active categories.
        /// </summary>
        [HttpGet("activas")]
        public async Task<ActionResult<IReadOnlyList<CategoriaResponseDto>>> GetActivas(
            CancellationToken cancellationToken = default)
        {
            var categorias = await _categoriaService.GetActivasAsync(cancellationToken);
            return Ok(categorias);
        }

        /// <summary>
        /// Gets a category by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaResponseDto>> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var categoria = await _categoriaService.GetByIdAsync(id, cancellationToken);
            if (categoria == null)
                return NotFound();

            return Ok(categoria);
        }

        /// <summary>
        /// Gets products for a category.
        /// </summary>
        [HttpGet("{id}/productos")]
        public async Task<ActionResult<IReadOnlyList<ProductoResponseDto>>> GetProductos(
            int id,
            CancellationToken cancellationToken = default)
        {
            var productos = await _categoriaService.GetProductosAsync(id, cancellationToken);
            return Ok(productos);
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CategoriaResponseDto>> Crear(
            [FromBody] CategoriaCrearRequest dto,
            CancellationToken cancellationToken = default)
        {
            var categoria = await _categoriaService.CrearAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<CategoriaResponseDto>> Actualizar(
            int id,
            [FromBody] CategoriaActualizarRequest dto,
            CancellationToken cancellationToken = default)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var categoria = await _categoriaService.ActualizarAsync(dto, cancellationToken);
            if (categoria == null)
                return NotFound();

            return Ok(categoria);
        }

        /// <summary>
        /// Deletes a category (soft delete).
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(
            int id,
            CancellationToken cancellationToken = default)
        {
            var success = await _categoriaService.EliminarAsync(id, cancellationToken);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}