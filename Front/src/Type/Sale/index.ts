import type { PageQuery } from '../Common'

export interface SaleDetailRequest {
  productoId: number
  cantidad: number
}

export interface SaleDetail {
  id: number
  ventaId: number
  productoId: number
  cantidad: number
  precioUnitario: number
  subtotal: number
}

export interface Sale {
  id: number
  fecha: string
  subtotal: number
  impuesto: number
  total: number
  metodoPago: number | null
  estado: number | null
  cajaId: number
  usuarioId: number
  clienteId: number | null
}

export interface SaleRequest {
  fecha?: string
  metodoPago?: number | null
  estado?: number | null
  usuarioId: number
  cajaId: number
  clienteId?: number | null
  detalles: SaleDetailRequest[]
}

export interface SaleDetailUpdateRequest extends SaleDetailRequest {
  id: number
}

export interface SaleUpdateRequest {
  id: number
  fecha: string
  metodoPago: number | null
  estado: number | null
  usuarioId: number
  cajaId: number
  clienteId: number | null
  detalles?: SaleDetailUpdateRequest[] | null
}

export interface SaleQuery extends PageQuery {}