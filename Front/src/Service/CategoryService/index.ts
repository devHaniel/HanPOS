import { apiClient } from '../ApiClient'
import { createCrudService } from '../CrudService'
import type { Category, CategoryRequest } from '@/Type/Category'
import type { Product } from '@/Type/Product'

const crud = createCrudService<Category, CategoryRequest>('/categoria', '/categoria/activas')

export const categoryService = {
  ...crud,
  list() {
    return apiClient.get<Category[]>('/categoria/activas')
  },
  getProducts(categoryId: number) {
    return apiClient.get<Product[]>(`/categoria/${categoryId}/productos`)
  },
}