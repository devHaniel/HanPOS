import { apiClient } from '../ApiClient'
import { createCrudService } from '../CrudService'
import type { PagedResult } from '@/Type/Common'
import type { CashRegister, CashRegisterQuery, CashRegisterRequest, CashRegisterUpdateRequest } from '@/Type/CashRegister'
import type { CashMovement } from '@/Type/CashMovement'

const crud = createCrudService<CashRegister, CashRegisterRequest, CashRegisterUpdateRequest>('/caja')

export const cashRegisterService = {
  ...crud,
  list(query?: CashRegisterQuery) {
    const params = new URLSearchParams()
    if (query?.pagina) params.set('pagina', String(query.pagina))
    if (query?.cantidad) params.set('cantidad', String(query.cantidad))
    if (query?.fechaDesde) params.set('fechaDesde', query.fechaDesde)
    if (query?.fechaHasta) params.set('fechaHasta', query.fechaHasta)
    return apiClient.get<PagedResult<CashRegister>>(`/caja?${params.toString()}`)
  },
  getOpen() {
    return apiClient.get<CashRegister>('/caja/abierta')
  },
  close(id: number, montoFinal?: number, observaciones?: string) {
    return apiClient.post<CashRegister>(`/caja/${id}/cerrar`, { montoFinal, observaciones })
  },
  getMovements(id: number) {
    return apiClient.get<CashMovement[]>(`/caja/${id}/movimientos`)
  },
  getExpectedClose(id: number) {
    return apiClient.get<{ 
      montoInicial: number
      ventasEfectivo: number
      entradas: number
      salidas: number
      montoEsperado: number
      ventasTarjeta: number
      ventasTransferencia: number
    }>(`/caja/${id}/esperado-cierre`)
  },
}