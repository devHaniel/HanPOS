namespace Api.DTOs.Reportes
{
    public class ReporteStockCriticoDto
    {
        public int ProductoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal Deficit { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal ValorReposicion { get; set; }
        public bool Agotado => StockActual <= 0;
        public bool Critico => StockActual > 0 && StockActual <= StockMinimo;
    }
}