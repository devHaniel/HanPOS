import { apiClient } from '../ApiClient'
import type { Negocio, NegocioRequest } from '@/Type/Negocio'

export const negocioService = {
  async get(): Promise<Negocio | null> {
    try {
      return await apiClient.get<Negocio>('/negocio')
    } catch (error: any) {
      if (error.status === 404) return null
      throw error
    }
  },

  async create(request: NegocioRequest): Promise<Negocio> {
    return apiClient.post<Negocio>('/negocio', request)
  },

  async update(id: number, request: NegocioRequest): Promise<Negocio> {
    return apiClient.put<Negocio>(`/negocio/${id}`, { ...request, id })
  },

  async exists(): Promise<boolean> {
    return apiClient.get<boolean>('/negocio/existe')
  }
}