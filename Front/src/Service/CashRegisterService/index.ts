import { apiClient } from '../ApiClient'
import { createCrudService } from '../CrudService'
import type { PagedResult } from '@/Type/Common'
import type { CashRegister, CashRegisterQuery, CashRegisterRequest, CashRegisterUpdateRequest } from '@/Type/CashRegister'
import type { CashMovement } from '@/Type/CashMovement'

const crud = createCrudService<CashRegister, CashRegisterRequest, CashRegisterUpdateRequest>('/caja')

export const cashRegisterService = {
  ...crud,
  list(query?: CashRegisterQuery) {
    return apiClient.get<PagedResult<CashRegister>>('/caja', query)
  },
  getOpen() {
    return apiClient.get<CashRegister>('/caja/abierta')
  },
  close(id: number) {
    return apiClient.post<CashRegister>(`/caja/${id}/cerrar`)
  },
  getMovements(id: number) {
    return apiClient.get<CashMovement[]>(`/caja/${id}/movimientos`)
  },
}