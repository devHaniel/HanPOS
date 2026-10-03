export interface PageQuery {
  pagina?: number
  cantidad?: number
}

export interface PagedResult<T> {
  items: T[]
  pagina: number
  cantidad: number
  total: number
  totalPaginas: number
}

export interface ProblemDetails {
  type?: string
  title?: string
  status: number
  detail?: string
  instance?: string
  traceId?: string
  errors?: Record<string, string[]>
}

export type ListResult<T> = T[] | PagedResult<T>