import { apiClient } from '../ApiClient'
import { createCrudService } from '../CrudService'
import type { ManagedUser, UserCreateRequest, UserUpdateRequest } from '@/Type/User'
import type { Sale } from '@/Type/Sale'
import type { Purchase } from '@/Type/Purchase'
import type { CashRegister } from '@/Type/CashRegister'

const crud = createCrudService<ManagedUser, UserCreateRequest, UserUpdateRequest>('/usuario', '/usuario/activos')

export const userService = {
  ...crud,
  list() {
    return apiClient.get<ManagedUser[]>('/usuario/activos')
  },
  getByUsername(username: string) {
    return apiClient.get<ManagedUser>(`/usuario/username/${encodeURIComponent(username)}`)
  },
  getSales(userId: number) {
    return apiClient.get<Sale[]>(`/usuario/${userId}/ventas`)
  },
  getPurchases(userId: number) {
    return apiClient.get<Purchase[]>(`/usuario/${userId}/compras`)
  },
  getCashRegisters(userId: number) {
    return apiClient.get<CashRegister[]>(`/usuario/${userId}/cajas`)
  },
}