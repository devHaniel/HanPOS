namespace Api.DTOs.Reportes
{
    public class ReporteKardexDto
    {
        public int ProductoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public List<KardexMovimientoDto> Movimientos { get; set; } = new();
        public decimal StockFinal { get; set; }
    }

    public class KardexMovimientoDto
    {
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = string.Empty; // VENTA, COMPRA, AJUSTE, INICIAL
        public string Referencia { get; set; } = string.Empty; // #Venta, #Compra, etc.
        public decimal Entrada { get; set; }
        public decimal Salida { get; set; }
        public decimal Saldo { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal CostoPromedio { get; set; }
    }
}