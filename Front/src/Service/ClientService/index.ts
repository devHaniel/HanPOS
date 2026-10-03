import { apiClient } from '../ApiClient'
import { createCrudService } from '../CrudService'
import type { Client, ClientRequest } from '@/Type/Client'
import type { Sale } from '@/Type/Sale'

const crud = createCrudService<Client, ClientRequest>('/cliente')

export const clientService = {
  ...crud,
  list() {
    return apiClient.get<Client[]>('/cliente')
  },
  getByRtn(rtn: string) {
    return apiClient.get<Client>(`/cliente/rtn/${encodeURIComponent(rtn)}`)
  },
  getSales(clientId: number) {
    return apiClient.get<Sale[]>(`/cliente/${clientId}/ventas`)
  },
}