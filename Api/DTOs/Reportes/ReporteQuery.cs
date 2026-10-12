namespace Api.DTOs.Reportes
{
    public class ReporteQuery
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? CategoriaId { get; set; }
        public int? ProveedorId { get; set; }
        public int? UsuarioId { get; set; }
        public int? CajaId { get; set; }
        public int Pagina { get; set; } = 1;
        public int Cantidad { get; set; } = 50;
    }
}