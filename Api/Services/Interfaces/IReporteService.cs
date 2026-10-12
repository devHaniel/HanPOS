using Api.DTOs.Reportes;
using Api.DTOs.Paginacion;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services.Interfaces
{
    public interface IReporteService
    {
        // Resumen de ventas
        Task<ReporteVentasResumenDto> GetVentasResumenAsync(DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken = default);

        // Ventas por producto
        Task<PagedResult<ReporteVentasPorProductoDto>> GetVentasPorProductoAsync(ReporteQuery query, CancellationToken cancellationToken = default);

        // Stock crítico
        Task<List<ReporteStockCriticoDto>> GetStockCriticoAsync(CancellationToken cancellationToken = default);

        // Kardex de un producto
        Task<ReporteKardexDto> GetKardexAsync(int productoId, DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken = default);

        // Impuestos
        Task<ReporteImpuestosDto> GetImpuestosAsync(DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken = default);

        // Resumen de compras
        Task<ReporteComprasResumenDto> GetComprasResumenAsync(DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken = default);

        // Ventas por vendedor
        Task<List<ReporteVentasPorVendedorDto>> GetVentasPorVendedorAsync(DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken = default);

        // Ventas por hora (heatmap)
        Task<List<ReporteVentasPorHoraDto>> GetVentasPorHoraAsync(DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken = default);

        // Top productos más vendidos
        Task<List<ReporteVentasPorProductoDto>> GetTopProductosAsync(int top, DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken = default);
    }

    public class ReporteVentasPorVendedorDto
    {
        public int UsuarioId { get; set; }
        public string NombreVendedor { get; set; } = string.Empty;
        public int TotalVentas { get; set; }
        public decimal TotalIngresos { get; set; }
        public decimal TicketPromedio { get; set; }
    }

    public class ReporteVentasPorHoraDto
    {
        public int Hora { get; set; }
        public int CantidadVentas { get; set; }
        public decimal TotalIngresos { get; set; }
    }
}