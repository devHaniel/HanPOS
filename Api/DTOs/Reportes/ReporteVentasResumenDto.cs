namespace Api.DTOs.Reportes
{
    public class ReporteVentasResumenDto
    {
        public int TotalVentas { get; set; }
        public int VentasCompletadas { get; set; }
        public int VentasCanceladas { get; set; }
        public decimal TotalIngresos { get; set; }
        public decimal TotalImpuestos { get; set; }
        public decimal Efectivo { get; set; }
        public decimal Tarjeta { get; set; }
        public decimal Transferencia { get; set; }
        public decimal TicketPromedio { get; set; }
    }
}