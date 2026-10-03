import { apiClient } from '../ApiClient'
import { createCrudService } from '../CrudService'
import type { PagedResult } from '@/Type/Common'
import type { Product, ProductQuery, ProductRequest } from '@/Type/Product'

const crud = createCrudService<Product, ProductRequest>('/producto', '/producto/activos')

export const productService = {
  ...crud,
  list(query?: ProductQuery) {
    return apiClient.get<PagedResult<Product>>('/producto/activos', query)
  },
  getByCode(code: string) {
    return apiClient.get<Product>(`/producto/codigo/${encodeURIComponent(code)}`)
  },
  getSales(productId: number) {
    return apiClient.get<unknown[]>(`/producto/${productId}/ventas`)
  },
  getPurchases(productId: number) {
    return apiClient.get<unknown[]>(`/producto/${productId}/compras`)
  },
}