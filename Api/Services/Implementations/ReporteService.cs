using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Data;
using Api.DTOs.Reportes;
using Api.DTOs.Paginacion;
using Api.Models.Entities;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Implementations
{
    public class ReporteService : IReporteService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly ICompraRepository _compraRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IDetalleVentaRepository _detalleVentaRepository;
        private readonly IDetalleCompraRepository _detalleCompraRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICajaRepository _cajaRepository;
        private readonly AppDbContext _context;

        public ReporteService(
            IVentaRepository ventaRepository,
            ICompraRepository compraRepository,
            IProductoRepository productoRepository,
            IDetalleVentaRepository detalleVentaRepository,
            IDetalleCompraRepository detalleCompraRepository,
            IUsuarioRepository usuarioRepository,
            ICajaRepository cajaRepository,
            AppDbContext context)
        {
            _ventaRepository = ventaRepository;
            _compraRepository = compraRepository;
            _productoRepository = productoRepository;
            _detalleVentaRepository = detalleVentaRepository;
            _detalleCompraRepository = detalleCompraRepository;
            _usuarioRepository = usuarioRepository;
            _cajaRepository = cajaRepository;
            _context = context;
        }

        public async Task<ReporteVentasResumenDto> GetVentasResumenAsync(DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken = default)
        {
            var query = _ventaRepository.GetQueryable();

            if (fechaDesde.HasValue)
            {
                var desde = fechaDesde.Value.Date;
                query = query.Where(v => v.Fecha.Date >= desde);
            }

            if (fechaHasta.HasValue)
            {
                var hasta = fechaHasta.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(v => v.Fecha <= hasta);
            }

            var ventas = await query.ToListAsync(cancellationToken);

            var completadas = ventas.Where(v => v.Estado == EstadoVenta.Completada).ToList();
            var canceladas = ventas.Where(v => v.Estado == EstadoVenta.Cancelada).ToList();

            return new ReporteVentasResumenDto
            {
                TotalVentas = ventas.Count,
                VentasCompletadas = completadas.Count,
                VentasCanceladas = canceladas.Count,
                TotalIngresos = completadas.Sum(v => v.Total),
                TotalImpuestos = completadas.Sum(v => v.Impuesto),
                Efectivo = completadas.Where(v => v.MetodoPago == MetodoPago.Efectivo).Sum(v => v.Total),
                Tarjeta = completadas.Where(v => v.MetodoPago == MetodoPago.Tarjeta).Sum(v => v.Total),
                Transferencia = completadas.Where(v => v.MetodoPago == MetodoPago.Transferencia).Sum(v => v.Total),
                TicketPromedio = completadas.Any() ? completadas.Average(v => v.Total) : 0
            };
        }

        public async Task<PagedResult<ReporteVentasPorProductoDto>> GetVentasPorProductoAsync(ReporteQuery query, CancellationToken cancellationToken = default)
        {
            var ventaQuery = _ventaRepository.GetQueryable()
                .Where(v => v.Estado == EstadoVenta.Completada);

            if (query.FechaDesde.HasValue)
            {
                var desde = query.FechaDesde.Value.Date;
                ventaQuery = ventaQuery.Where(v => v.Fecha.Date >= desde);
            }

            if (query.FechaHasta.HasValue)
            {
                var hasta = query.FechaHasta.Value.Date.AddDays(1).AddTicks(-1);
                ventaQuery = ventaQuery.Where(v => v.Fecha <= hasta);
            }

            var ventaIds = await ventaQuery.Select(v => v.Id).ToListAsync(cancellationToken);

            if (!ventaIds.Any())
            {
                return new PagedResult<ReporteVentasPorProductoDto>
                {
                    Items = new List<ReporteVentasPorProductoDto>(),
                    Pagina = query.Pagina,
                    Cantidad = query.Cantidad,
                    Total = 0
                };
            }

            // Use context directly for complex includes
            var detallesQuery = (IQueryable<DetalleVenta>)_context.DetallesVenta
                .Where(d => ventaIds.Contains(d.VentaId))
                .Include(d => d.Producto)
                    .ThenInclude(p => p.Categoria);

            if (query.CategoriaId.HasValue)
            {
                detallesQuery = detallesQuery.Where(d => d.Producto.CategoriaId == query.CategoriaId.Value);
            }

            var detalles = await detallesQuery.ToListAsync(cancellationToken);

            var grouped = detalles
                .GroupBy(d => new { d.ProductoId, d.Producto.Codigo, d.Producto.Nombre, Categoria = d.Producto.Categoria != null ? d.Producto.Categoria.Nombre : "Sin categoría", d.Producto.Stock, d.Producto.PrecioVenta, d.Producto.PrecioCompra })
                .Select(g => new ReporteVentasPorProductoDto
                {
                    ProductoId = g.Key.ProductoId,
                    Codigo = g.Key.Codigo,
                    Nombre = g.Key.Nombre,
                    Categoria = g.Key.Categoria,
                    CantidadVendida = g.Sum(x => x.Cantidad),
                    PrecioPromedio = g.Average(x => x.PrecioUnitario),
                    TotalVendido = g.Sum(x => x.Subtotal),
                    MargenEstimado = g.Sum(x => (int)((x.PrecioUnitario - g.Key.PrecioCompra) * x.Cantidad)),
                    StockActual = g.Key.Stock
                })
                .OrderByDescending(x => x.TotalVendido)
                .ToList();

            var total = grouped.Count;
            var items = grouped
                .Skip((query.Pagina - 1) * query.Cantidad)
                .Take(query.Cantidad)
                .ToList();

            return new PagedResult<ReporteVentasPorProductoDto>
            {
                Items = items,
                Pagina = query.Pagina,
                Cantidad = query.Cantidad,
                Total = total
            };
        }

        public async Task<List<ReporteStockCriticoDto>> GetStockCriticoAsync(CancellationToken cancellationToken = default)
        {
            var productos = await _productoRepository.GetQueryable()
                .Include(p => p.Categoria)
                .Where(p => p.Activo && p.Stock <= p.StockMinimo)
                .OrderBy(p => p.Stock)
                .ToListAsync(cancellationToken);

            return productos.Select(p => new ReporteStockCriticoDto
            {
                ProductoId = p.Id,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Categoria = p.Categoria?.Nombre ?? "Sin categoría",
                StockActual = p.Stock,
                StockMinimo = p.StockMinimo,
                Deficit = p.StockMinimo - p.Stock,
                PrecioVenta = p.PrecioVenta,
                ValorReposicion = (p.StockMinimo - p.Stock) * p.PrecioCompra
            }).ToList();
        }

        public async Task<ReporteKardexDto> GetKardexAsync(int productoId, DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken = default)
        {
            var producto = await _productoRepository.GetByIdAsync(productoId, cancellationToken);
            if (producto == null)
                return new ReporteKardexDto { ProductoId = productoId };

            var movimientos = new List<KardexMovimientoDto>();

            // Ventas (salidas)
            var ventaQuery = (IQueryable<Venta>)_ventaRepository.GetQueryable()
                .Where(v => v.Estado == EstadoVenta.Completada)
                .Include(v => v.Detalles.Where(d => d.ProductoId == productoId));

            if (fechaDesde.HasValue)
            {
                var desde = fechaDesde.Value.Date;
                ventaQuery = ventaQuery.Where(v => v.Fecha.Date >= desde);
            }

            if (fechaHasta.HasValue)
            {
                var hasta = fechaHasta.Value.Date.AddDays(1).AddTicks(-1);
                ventaQuery = ventaQuery.Where(v => v.Fecha <= hasta);
            }

            var ventas = await ventaQuery.ToListAsync(cancellationToken);

            foreach (var venta in ventas.OrderBy(v => v.Fecha))
            {
                var detalle = venta.Detalles.FirstOrDefault(d => d.ProductoId == productoId);
                if (detalle != null)
                {
                    movimientos.Add(new KardexMovimientoDto
                    {
                        Fecha = venta.Fecha,
                        Tipo = "VENTA",
                        Referencia = $"Venta #{venta.Id}",
                        Entrada = 0,
                        Salida = detalle.Cantidad,
                        Saldo = 0,
                        CostoUnitario = detalle.PrecioUnitario,
                        CostoPromedio = 0
                    });
                }
            }

            // Compras (entradas)
            var compraQuery = (IQueryable<Compra>)_compraRepository.GetQueryable()
                .Where(c => c.Estado == EstadoCompra.Completada)
                .Include(c => c.Detalles.Where(d => d.ProductoId == productoId));

            if (fechaDesde.HasValue)
            {
                var desde = fechaDesde.Value.Date;
                compraQuery = compraQuery.Where(c => c.Fecha.Date >= desde);
            }

            if (fechaHasta.HasValue)
            {
                var hasta = fechaHasta.Value.Date.AddDays(1).AddTicks(-1);
                compraQuery = compraQuery.Where(c => c.Fecha <= hasta);
            }

            var compras = await compraQuery.ToListAsync(cancellationToken);

            foreach (var compra in compras.OrderBy(c => c.Fecha))
            {
                var detalle = compra.Detalles.FirstOrDefault(d => d.ProductoId == productoId);
                if (detalle != null)
                {
                    movimientos.Add(new KardexMovimientoDto
                    {
                        Fecha = compra.Fecha,
                        Tipo = "COMPRA",
                        Referencia = $"Compra #{compra.Id}",
                        Entrada = detalle.Cantidad,
                        Salida = 0,
                        Saldo = 0,
                        CostoUnitario = detalle.PrecioUnitario,
                        CostoPromedio = 0
                    });
                }
            }

            // Ordenar por fecha
            movimientos = movimientos.OrderBy(m => m.Fecha).ToList();

            // Recalcular saldos en orden cronológico
            decimal saldo = 0;
            foreach (var m in movimientos)
            {
                saldo += m.Entrada - m.Salida;
                m.Saldo = saldo;
            }

            return new ReporteKardexDto
            {
                ProductoId = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Movimientos = movimientos,
                StockFinal = saldo
            };
        }

        public async Task<ReporteImpuestosDto> GetImpuestosAsync(DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken = default)
        {
            var query = _ventaRepository.GetQueryable()
                .Where(v => v.Estado == EstadoVenta.Completada);

            if (fechaDesde.HasValue)
            {
                var desde = fechaDesde.Value.Date;
                query = query.Where(v => v.Fecha.Date >= desde);
            }

            if (fechaHasta.HasValue)
            {
                var hasta = fechaHasta.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(v => v.Fecha <= hasta);
            }

            var ventas = await query.ToListAsync(cancellationToken);

            var reporte = new ReporteImpuestosDto
            {
                FechaDesde = fechaDesde ?? DateTime.MinValue,
                FechaHasta = fechaHasta ?? DateTime.MaxValue,
                TotalVentasGravadas = ventas.Sum(v => v.Subtotal),
                TotalIVA = ventas.Sum(v => v.Impuesto),
                TotalVentasExentas = 0
            };

            // Agrupar por mes
            reporte.DetallePorPeriodo = ventas
                .GroupBy(v => new { v.Fecha.Year, v.Fecha.Month })
                .Select(g => new ImpuestoPorPeriodoDto
                {
                    Periodo = $"{g.Key.Year}-{g.Key.Month:D2}",
                    BaseGravada = g.Sum(v => v.Subtotal),
                    IVA = g.Sum(v => v.Impuesto),
                    CantidadVentas = g.Count()
                })
                .OrderBy(x => x.Periodo)
                .ToList();

            // Agrupar por método de pago
            reporte.DetallePorMetodoPago = ventas
                .GroupBy(v => v.MetodoPago)
                .Select(g => new ImpuestoPorMetodoPagoDto
                {
                    MetodoPago = g.Key switch
                    {
                        MetodoPago.Efectivo => "Efectivo",
                        MetodoPago.Tarjeta => "Tarjeta",
                        MetodoPago.Transferencia => "Transferencia",
                        _ => "Sin definir"
                    },
                    BaseGravada = g.Sum(v => v.Subtotal),
                    IVA = g.Sum(v => v.Impuesto),
                    CantidadVentas = g.Count()
                })
                .ToList();

            return reporte;
        }

        public async Task<ReporteComprasResumenDto> GetComprasResumenAsync(DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken = default)
        {
            var query = _compraRepository.GetQueryable();

            if (fechaDesde.HasValue)
            {
                var desde = fechaDesde.Value.Date;
                query = query.Where(c => c.Fecha.Date >= desde);
            }

            if (fechaHasta.HasValue)
            {
                var hasta = fechaHasta.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(c => c.Fecha <= hasta);
            }

            var compras = await query.ToListAsync(cancellationToken);

            var completadas = compras.Where(c => c.Estado == EstadoCompra.Completada).ToList();
            var pendientes = compras.Where(c => c.Estado == EstadoCompra.Pendiente).ToList();
            var canceladas = compras.Where(c => c.Estado == EstadoCompra.Cancelada).ToList();

            return new ReporteComprasResumenDto
            {
                TotalCompras = compras.Count,
                ComprasCompletadas = completadas.Count,
                ComprasRegistradas = pendientes.Count,
                TotalGastado = completadas.Sum(c => c.Total),
                TotalImpuestos = completadas.Sum(c => c.Impuesto),
                TicketPromedio = completadas.Any() ? completadas.Average(c => c.Total) : 0,
                ProveedoresUnicos = completadas.Select(c => c.ProveedorId).Distinct().Count()
            };
        }

        public async Task<List<ReporteVentasPorVendedorDto>> GetVentasPorVendedorAsync(DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken = default)
        {
            var query = _ventaRepository.GetQueryable()
                .Where(v => v.Estado == EstadoVenta.Completada);

            if (fechaDesde.HasValue)
            {
                var desde = fechaDesde.Value.Date;
                query = query.Where(v => v.Fecha.Date >= desde);
            }

            if (fechaHasta.HasValue)
            {
                var hasta = fechaHasta.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(v => v.Fecha <= hasta);
            }

            var ventas = await query.ToListAsync(cancellationToken);
            var usuarios = await _usuarioRepository.GetQueryable().ToListAsync(cancellationToken);
            var usuarioDict = usuarios.ToDictionary(u => u.Id, u => u.Nombre);

            return ventas
                .GroupBy(v => v.UsuarioId)
                .Select(g => new ReporteVentasPorVendedorDto
                {
                    UsuarioId = g.Key,
                    NombreVendedor = usuarioDict.TryGetValue(g.Key, out var nombre) ? nombre : $"Usuario #{g.Key}",
                    TotalVentas = g.Count(),
                    TotalIngresos = g.Sum(v => v.Total),
                    TicketPromedio = g.Average(v => v.Total)
                })
                .OrderByDescending(x => x.TotalIngresos)
                .ToList();
        }

        public async Task<List<ReporteVentasPorHoraDto>> GetVentasPorHoraAsync(DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken = default)
        {
            var query = _ventaRepository.GetQueryable()
                .Where(v => v.Estado == EstadoVenta.Completada);

            if (fechaDesde.HasValue)
            {
                var desde = fechaDesde.Value.Date;
                query = query.Where(v => v.Fecha.Date >= desde);
            }

            if (fechaHasta.HasValue)
            {
                var hasta = fechaHasta.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(v => v.Fecha <= hasta);
            }

            var ventas = await query.ToListAsync(cancellationToken);

            return ventas
                .GroupBy(v => v.Fecha.Hour)
                .Select(g => new ReporteVentasPorHoraDto
                {
                    Hora = g.Key,
                    CantidadVentas = g.Count(),
                    TotalIngresos = g.Sum(v => v.Total)
                })
                .OrderBy(x => x.Hora)
                .ToList();
        }

        public async Task<List<ReporteVentasPorProductoDto>> GetTopProductosAsync(int top, DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken = default)
        {
            var query = new ReporteQuery
            {
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                Pagina = 1,
                Cantidad = top
            };

            var result = await GetVentasPorProductoAsync(query, cancellationToken);
            return result.Items;
        }
    }
}