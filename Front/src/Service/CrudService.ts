import { apiClient } from './ApiClient'
import type { ListResult, PageQuery } from '@/Type/Common'

export interface CrudService<T, TCreate, TUpdate = TCreate> {
  list(query?: PageQuery): Promise<ListResult<T>>
  getById(id: number): Promise<T>
  create(request: TCreate): Promise<T>
  update(id: number, request: TUpdate): Promise<T>
  remove(id: number): Promise<void>
}

export function createCrudService<T, TCreate, TUpdate = TCreate>(
  endpoint: string,
  listEndpoint = endpoint,
): CrudService<T, TCreate, TUpdate> {
  return {
    list: (query) => apiClient.get<ListResult<T>>(listEndpoint, query ? {
      pagina: query.pagina,
      cantidad: query.cantidad,
    } : undefined),
    getById: (id) => apiClient.get<T>(`${endpoint}/${id}`),
    create: (request) => apiClient.post<T>(endpoint, request),
    update: (id, request) => apiClient.put<T>(`${endpoint}/${id}`, {
      ...(request as object),
      id,
    }),
    remove: (id) => apiClient.delete<void>(`${endpoint}/${id}`),
  }
}