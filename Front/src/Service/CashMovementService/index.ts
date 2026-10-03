import { apiClient } from '../ApiClient'
import { createCrudService } from '../CrudService'
import type { CashMovement, CashMovementRequest, CashMovementUpdateRequest } from '@/Type/CashMovement'

const crud = createCrudService<CashMovement, CashMovementRequest, CashMovementUpdateRequest>('/movimientocaja')

export const cashMovementService = {
  ...crud,
  list() {
    return apiClient.get<CashMovement[]>('/movimientocaja')
  },
  getByRegister(registerId: number) {
    return apiClient.get<CashMovement[]>(`/movimientocaja/por-caja/${registerId}`)
  },
}