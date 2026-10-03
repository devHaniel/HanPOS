export interface Category {
  id: number
  nombre: string
  descripcion: string | null
  activo: boolean
}

export interface CategoryRequest {
  nombre: string
  descripcion?: string | null
  activo: boolean
}