import { apiClient } from '../ApiClient'
import { createCrudService } from '../CrudService'
import type { PagedResult } from '@/Type/Common'
import type { Purchase, PurchaseDetail, PurchaseQuery, PurchaseRequest, PurchaseUpdateRequest } from '@/Type/Purchase'

const crud = createCrudService<Purchase, PurchaseRequest, PurchaseUpdateRequest>('/compra')

export const purchaseService = {
  ...crud,
  list(query?: PurchaseQuery) {
    return apiClient.get<PagedResult<Purchase>>('/compra', query)
  },
  getByDate(date: string) {
    return apiClient.get<Purchase[]>(`/compra/por-fecha/${encodeURIComponent(date)}`)
  },
  getByProvider(providerId: number) {
    return apiClient.get<Purchase[]>(`/compra/por-proveedor/${providerId}`)
  },
  getDetails(purchaseId: number) {
    return apiClient.get<PurchaseDetail[]>(`/compra/${purchaseId}/detalles`)
  },
}