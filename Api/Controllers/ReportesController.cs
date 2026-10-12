using Api.DTOs.Reportes;
using Api.DTOs.Paginacion;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for reports.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReportesController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        /// <summary>
        /// Gets sales summary report.
        /// </summary>
        [HttpGet("ventas/resumen")]
        public async Task<ActionResult<ReporteVentasResumenDto>> GetVentasResumen(
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            CancellationToken cancellationToken = default)
        {
            // Ensure DateTime has Utc kind for PostgreSQL
            var desde = fechaDesde?.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fechaDesde.Value, DateTimeKind.Utc) : fechaDesde?.ToUniversalTime();
            var hasta = fechaHasta?.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fechaHasta.Value, DateTimeKind.Utc) : fechaHasta?.ToUniversalTime();

            var result = await _reporteService.GetVentasResumenAsync(desde, hasta, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets sales by product report with pagination.
        /// </summary>
        [HttpGet("ventas/por-producto")]
        public async Task<ActionResult<PagedResult<ReporteVentasPorProductoDto>>> GetVentasPorProducto(
            [FromQuery] ReporteQuery query,
            CancellationToken cancellationToken = default)
        {
            // Ensure DateTime has Utc kind for PostgreSQL
            if (query.FechaDesde.HasValue && query.FechaDesde.Value.Kind == DateTimeKind.Unspecified)
                query.FechaDesde = DateTime.SpecifyKind(query.FechaDesde.Value, DateTimeKind.Utc);
            if (query.FechaHasta.HasValue && query.FechaHasta.Value.Kind == DateTimeKind.Unspecified)
                query.FechaHasta = DateTime.SpecifyKind(query.FechaHasta.Value, DateTimeKind.Utc);

            var result = await _reporteService.GetVentasPorProductoAsync(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets critical stock report.
        /// </summary>
        [HttpGet("stock/critico")]
        public async Task<ActionResult<List<ReporteStockCriticoDto>>> GetStockCritico(
            CancellationToken cancellationToken = default)
        {
            var result = await _reporteService.GetStockCriticoAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets kardex for a specific product.
        /// </summary>
        [HttpGet("kardex/{productoId}")]
        public async Task<ActionResult<ReporteKardexDto>> GetKardex(
            int productoId,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            CancellationToken cancellationToken = default)
        {
            // Ensure DateTime has Utc kind for PostgreSQL
            var desde = fechaDesde?.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fechaDesde.Value, DateTimeKind.Utc) : fechaDesde?.ToUniversalTime();
            var hasta = fechaHasta?.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fechaHasta.Value, DateTimeKind.Utc) : fechaHasta?.ToUniversalTime();

            var result = await _reporteService.GetKardexAsync(productoId, desde, hasta, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets taxes report.
        /// </summary>
        [HttpGet("impuestos")]
        public async Task<ActionResult<ReporteImpuestosDto>> GetImpuestos(
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            CancellationToken cancellationToken = default)
        {
            // Ensure DateTime has Utc kind for PostgreSQL
            var desde = fechaDesde?.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fechaDesde.Value, DateTimeKind.Utc) : fechaDesde?.ToUniversalTime();
            var hasta = fechaHasta?.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fechaHasta.Value, DateTimeKind.Utc) : fechaHasta?.ToUniversalTime();

            var result = await _reporteService.GetImpuestosAsync(desde, hasta, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets purchases summary report.
        /// </summary>
        [HttpGet("compras/resumen")]
        public async Task<ActionResult<ReporteComprasResumenDto>> GetComprasResumen(
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            CancellationToken cancellationToken = default)
        {
            // Ensure DateTime has Utc kind for PostgreSQL
            var desde = fechaDesde?.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fechaDesde.Value, DateTimeKind.Utc) : fechaDesde?.ToUniversalTime();
            var hasta = fechaHasta?.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fechaHasta.Value, DateTimeKind.Utc) : fechaHasta?.ToUniversalTime();

            var result = await _reporteService.GetComprasResumenAsync(desde, hasta, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets sales by vendor report.
        /// </summary>
        [HttpGet("ventas/por-vendedor")]
        public async Task<ActionResult<List<ReporteVentasPorVendedorDto>>> GetVentasPorVendedor(
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            CancellationToken cancellationToken = default)
        {
            // Ensure DateTime has Utc kind for PostgreSQL
            var desde = fechaDesde?.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fechaDesde.Value, DateTimeKind.Utc) : fechaDesde?.ToUniversalTime();
            var hasta = fechaHasta?.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fechaHasta.Value, DateTimeKind.Utc) : fechaHasta?.ToUniversalTime();

            var result = await _reporteService.GetVentasPorVendedorAsync(desde, hasta, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets sales by hour report (heatmap).
        /// </summary>
        [HttpGet("ventas/por-hora")]
        public async Task<ActionResult<List<ReporteVentasPorHoraDto>>> GetVentasPorHora(
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            CancellationToken cancellationToken = default)
        {
            // Ensure DateTime has Utc kind for PostgreSQL
            var desde = fechaDesde?.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fechaDesde.Value, DateTimeKind.Utc) : fechaDesde?.ToUniversalTime();
            var hasta = fechaHasta?.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fechaHasta.Value, DateTimeKind.Utc) : fechaHasta?.ToUniversalTime();

            var result = await _reporteService.GetVentasPorHoraAsync(desde, hasta, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets top selling products.
        /// </summary>
        [HttpGet("ventas/top-productos")]
        public async Task<ActionResult<List<ReporteVentasPorProductoDto>>> GetTopProductos(
            int top = 10,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            CancellationToken cancellationToken = default)
        {
            // Ensure DateTime has Utc kind for PostgreSQL
            var desde = fechaDesde?.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fechaDesde.Value, DateTimeKind.Utc) : fechaDesde?.ToUniversalTime();
            var hasta = fechaHasta?.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fechaHasta.Value, DateTimeKind.Utc) : fechaHasta?.ToUniversalTime();

            var result = await _reporteService.GetTopProductosAsync(top, desde, hasta, cancellationToken);
            return Ok(result);
        }
    }
}