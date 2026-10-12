import { apiClient } from '../ApiClient'
import { createCrudService } from '../CrudService'
import type { PagedResult } from '@/Type/Common'
import type { Sale, SaleDetail, SaleQuery, SaleRequest, SaleUpdateRequest } from '@/Type/Sale'

const crud = createCrudService<Sale, SaleRequest, SaleUpdateRequest>('/venta')

export const saleService = {
  ...crud,
  list(query?: SaleQuery) {
    const params = new URLSearchParams()
    if (query?.pagina) params.set('pagina', String(query.pagina))
    if (query?.cantidad) params.set('cantidad', String(query.cantidad))
    if (query?.fechaDesde) params.set('fechaDesde', query.fechaDesde)
    if (query?.fechaHasta) params.set('fechaHasta', query.fechaHasta)
    return apiClient.get<PagedResult<Sale>>(`/venta?${params.toString()}`)
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