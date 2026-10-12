namespace Api.DTOs.Reportes
{
    public class ReporteImpuestosDto
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public decimal TotalVentasGravadas { get; set; }
        public decimal TotalIVA { get; set; }
        public decimal TotalVentasExentas { get; set; }
        public List<ImpuestoPorPeriodoDto> DetallePorPeriodo { get; set; } = new();
        public List<ImpuestoPorMetodoPagoDto> DetallePorMetodoPago { get; set; } = new();
    }

    public class ImpuestoPorPeriodoDto
    {
        public string Periodo { get; set; } = string.Empty; // YYYY-MM
        public decimal BaseGravada { get; set; }
        public decimal IVA { get; set; }
        public int CantidadVentas { get; set; }
    }

    public class ImpuestoPorMetodoPagoDto
    {
        public string MetodoPago { get; set; } = string.Empty;
        public decimal BaseGravada { get; set; }
        public decimal IVA { get; set; }
        public int CantidadVentas { get; set; }
    }
}