import { apiClient } from '../ApiClient'
import { createCrudService } from '../CrudService'
import type { Provider, ProviderRequest } from '@/Type/Provider'
import type { Purchase } from '@/Type/Purchase'

const crud = createCrudService<Provider, ProviderRequest>('/proveedor')

export const providerService = {
  ...crud,
  list() {
    return apiClient.get<Provider[]>('/proveedor')
  },
  getByRtn(rtn: string) {
    return apiClient.get<Provider>(`/proveedor/rtn/${encodeURIComponent(rtn)}`)
  },
  getPurchases(providerId: number) {
    return apiClient.get<Purchase[]>(`/proveedor/${providerId}/compras`)
  },
}