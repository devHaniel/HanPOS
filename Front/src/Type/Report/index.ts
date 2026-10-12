import type { PageQuery } from '../Common'

export interface ReporteVentasResumen {
  totalVentas: number
  ventasCompletadas: number
  ventasCanceladas: number
  totalIngresos: number
  totalImpuestos: number
  efectivo: number
  tarjeta: number
  transferencia: number
  ticketPromedio: number
}

export interface ReporteVentasPorProducto {
  productoId: number
  codigo: string
  nombre: string
  categoria: string
  cantidadVendida: number
  precioPromedio: number
  totalVendido: number
  margenEstimado: number
  stockActual: number
}

export interface ReporteStockCritico {
  productoId: number
  codigo: string
  nombre: string
  categoria: string
  stockActual: number
  stockMinimo: number
  deficit: number
  precioVenta: number
  valorReposicion: number
  agotado: boolean
  critico: boolean
}

export interface KardexMovimiento {
  fecha: string
  tipo: string
  referencia: string
  entrada: number
  salida: number
  saldo: number
  costoUnitario: number
  costoPromedio: number
}

export interface ReporteKardex {
  productoId: number
  codigo: string
  nombre: string
  movimientos: KardexMovimiento[]
  stockFinal: number
}

export interface ImpuestoPorPeriodo {
  periodo: string
  baseGravada: number
  iva: number
  cantidadVentas: number
}

export interface ImpuestoPorMetodoPago {
  metodoPago: string
  baseGravada: number
  iva: number
  cantidadVentas: number
}

export interface ReporteImpuestos {
  fechaDesde: string
  fechaHasta: string
  totalVentasGravadas: number
  totalIVA: number
  totalVentasExentas: number
  detallePorPeriodo: ImpuestoPorPeriodo[]
  detallePorMetodoPago: ImpuestoPorMetodoPago[]
}

export interface ReporteComprasResumen {
  totalCompras: number
  comprasCompletadas: number
  comprasRegistradas: number
  totalGastado: number
  totalImpuestos: number
  ticketPromedio: number
  proveedoresUnicos: number
}

export interface ReporteVentasPorVendedor {
  usuarioId: number
  nombreVendedor: string
  totalVentas: number
  totalIngresos: number
  ticketPromedio: number
}

export interface ReporteVentasPorHora {
  hora: number
  cantidadVentas: number
  totalIngresos: number
}

export interface ReporteQuery extends PageQuery {
  fechaDesde?: string
  fechaHasta?: string
  categoriaId?: number
  proveedorId?: number
  usuarioId?: number
  cajaId?: number
}