namespace Api.DTOs.Reportes
{
    public class ReporteVentasPorProductoDto
    {
        public int ProductoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal CantidadVendida { get; set; }
        public decimal PrecioPromedio { get; set; }
        public decimal TotalVendido { get; set; }
        public decimal MargenEstimado { get; set; }
        public decimal StockActual { get; set; }
    }
}