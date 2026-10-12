import { apiClient } from '../ApiClient'
import { createCrudService } from '../CrudService'
import type { PagedResult } from '@/Type/Common'
import type { Purchase, PurchaseDetail, PurchaseQuery, PurchaseRequest, PurchaseUpdateRequest } from '@/Type/Purchase'

const crud = createCrudService<Purchase, PurchaseRequest, PurchaseUpdateRequest>('/compra')

export const purchaseService = {
  ...crud,
  list(query?: PurchaseQuery) {
    const params = new URLSearchParams()
    if (query?.pagina) params.set('pagina', String(query.pagina))
    if (query?.cantidad) params.set('cantidad', String(query.cantidad))
    if (query?.fechaDesde) params.set('fechaDesde', query.fechaDesde)
    if (query?.fechaHasta) params.set('fechaHasta', query.fechaHasta)
    return apiClient.get<PagedResult<Purchase>>(`/compra?${params.toString()}`)
  },
  getByDate(date: string) {
    return apiClient.get<Purchase[]>(`/compra/por-fecha/${encodeURIComponent(date)}`)
  },
  getByProvider(providerId: number) {
    return apiClient.get<Purchase[]>(`/compra/por-proveedor/${providerId}`)
  },
  getByCashRegister(cajaId: number) {
    return apiClient.get<Purchase[]>(`/compra/por-caja/${cajaId}`)
  },
  getDetails(purchaseId: number) {
    return apiClient.get<PurchaseDetail[]>(`/compra/${purchaseId}/detalles`)
  },
}