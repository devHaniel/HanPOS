import type { PageQuery } from '../Common'

export interface Product {
  id: number
  codigo: string
  nombre: string
  precioVenta: number
  precioCompra: number
  stock: number
  stockMinimo: number
  activo: boolean
  categoriaId: number | null
}

export interface ProductRequest {
  codigo: string
  nombre: string
  precioVenta: number
  precioCompra: number
  stock: number
  stockMinimo: number
  activo: boolean
  categoriaId?: number | null
}

export interface ProductQuery extends PageQuery {}