namespace Api.DTOs.Reportes
{
    public class ReporteComprasResumenDto
    {
        public int TotalCompras { get; set; }
        public int ComprasCompletadas { get; set; }
        public int ComprasRegistradas { get; set; }
        public decimal TotalGastado { get; set; }
        public decimal TotalImpuestos { get; set; }
        public decimal TicketPromedio { get; set; }
        public int ProveedoresUnicos { get; set; }
    }
}