import { apiClient } from '../ApiClient'
import { createCrudService } from '../CrudService'
import type { PagedResult } from '@/Type/Common'
import type { Sale, SaleDetail, SaleQuery, SaleRequest, SaleUpdateRequest } from '@/Type/Sale'

const crud = createCrudService<Sale, SaleRequest, SaleUpdateRequest>('/venta')

export const saleService = {
  ...crud,
  list(query?: SaleQuery) {
    return apiClient.get<PagedResult<Sale>>('/venta', query)
  },
  getByDate(date: string) {
    return apiClient.get<Sale[]>(`/venta/por-fecha/${encodeURIComponent(date)}`)
  },
  getByUser(userId: number) {
    return apiClient.get<Sale[]>(`/venta/por-usuario/${userId}`)
  },
  getByCashRegister(cajaId: number) {
    return apiClient.get<Sale[]>(`/venta/por-caja/${cajaId}`)
  },
  getDetails(saleId: number) {
    return apiClient.get<SaleDetail[]>(`/venta/${saleId}/detalles`)
  },
}