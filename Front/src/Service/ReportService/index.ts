import { apiClient } from '../ApiClient'
import type { PagedResult } from '@/Type/Common'
import type {
  ReporteVentasResumen,
  ReporteVentasPorProducto,
  ReporteStockCritico,
  ReporteKardex,
  ReporteImpuestos,
  ReporteComprasResumen,
  ReporteVentasPorVendedor,
  ReporteVentasPorHora,
  ReporteQuery
} from '@/Type/Report'

export const reportService = {
  // Resumen de ventas
  getVentasResumen(fechaDesde?: string, fechaHasta?: string) {
    const params = new URLSearchParams()
    if (fechaDesde) params.set('fechaDesde', fechaDesde)
    if (fechaHasta) params.set('fechaHasta', fechaHasta)
    return apiClient.get<ReporteVentasResumen>(`/reportes/ventas/resumen?${params.toString()}`)
  },

  // Ventas por producto (paginado)
  getVentasPorProducto(query: ReporteQuery) {
    const params = new URLSearchParams()
    if (query.fechaDesde) params.set('fechaDesde', query.fechaDesde)
    if (query.fechaHasta) params.set('fechaHasta', query.fechaHasta)
    if (query.categoriaId) params.set('categoriaId', String(query.categoriaId))
    params.set('pagina', String(query.pagina || 1))
    params.set('cantidad', String(query.cantidad || 50))
    return apiClient.get<PagedResult<ReporteVentasPorProducto>>(`/reportes/ventas/por-producto?${params.toString()}`)
  },

  // Stock crítico
  getStockCritico() {
    return apiClient.get<ReporteStockCritico[]>(`/reportes/stock/critico`)
  },

  // Kardex de un producto
  getKardex(productoId: number, fechaDesde?: string, fechaHasta?: string) {
    const params = new URLSearchParams()
    if (fechaDesde) params.set('fechaDesde', fechaDesde)
    if (fechaHasta) params.set('fechaHasta', fechaHasta)
    return apiClient.get<ReporteKardex>(`/reportes/kardex/${productoId}?${params.toString()}`)
  },

  // Impuestos
  getImpuestos(fechaDesde?: string, fechaHasta?: string) {
    const params = new URLSearchParams()
    if (fechaDesde) params.set('fechaDesde', fechaDesde)
    if (fechaHasta) params.set('fechaHasta', fechaHasta)
    return apiClient.get<ReporteImpuestos>(`/reportes/impuestos?${params.toString()}`)
  },

  // Resumen de compras
  getComprasResumen(fechaDesde?: string, fechaHasta?: string) {
    const params = new URLSearchParams()
    if (fechaDesde) params.set('fechaDesde', fechaDesde)
    if (fechaHasta) params.set('fechaHasta', fechaHasta)
    return apiClient.get<ReporteComprasResumen>(`/reportes/compras/resumen?${params.toString()}`)
  },

  // Ventas por vendedor
  getVentasPorVendedor(fechaDesde?: string, fechaHasta?: string) {
    const params = new URLSearchParams()
    if (fechaDesde) params.set('fechaDesde', fechaDesde)
    if (fechaHasta) params.set('fechaHasta', fechaHasta)
    return apiClient.get<ReporteVentasPorVendedor[]>(`/reportes/ventas/por-vendedor?${params.toString()}`)
  },

  // Ventas por hora (heatmap)
  getVentasPorHora(fechaDesde?: string, fechaHasta?: string) {
    const params = new URLSearchParams()
    if (fechaDesde) params.set('fechaDesde', fechaDesde)
    if (fechaHasta) params.set('fechaHasta', fechaHasta)
    return apiClient.get<ReporteVentasPorHora[]>(`/reportes/ventas/por-hora?${params.toString()}`)
  },

  // Top productos más vendidos
  getTopProductos(top: number = 10, fechaDesde?: string, fechaHasta?: string) {
    const params = new URLSearchParams()
    params.set('top', String(top))
    if (fechaDesde) params.set('fechaDesde', fechaDesde)
    if (fechaHasta) params.set('fechaHasta', fechaHasta)
    return apiClient.get<ReporteVentasPorProducto[]>(`/reportes/ventas/top-productos?${params.toString()}`)
  }
}