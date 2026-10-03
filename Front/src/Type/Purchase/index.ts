import type { PageQuery } from '../Common'

export interface PurchaseDetailRequest {
  productoId: number
  cantidad: number
}

export interface PurchaseDetail {
  id: number
  compraId: number
  productoId: number
  cantidad: number
  precioUnitario: number
  subtotal: number
}

export interface Purchase {
  id: number
  fecha: string
  subtotal: number
  impuesto: number
  total: number
  estado: number
  cajaId: number
  usuarioId: number
  proveedorId: number
}

export interface PurchaseRequest {
  fecha?: string
  estado?: number
  usuarioId: number
  cajaId: number
  proveedorId: number
  detalles: PurchaseDetailRequest[]
}

export interface PurchaseDetailUpdateRequest extends PurchaseDetailRequest {
  id: number
}

export interface PurchaseUpdateRequest {
  id: number
  fecha: string
  estado: number
  usuarioId: number
  cajaId: number
  proveedorId: number
  detalles?: PurchaseDetailUpdateRequest[] | null
}

export interface PurchaseQuery extends PageQuery {}