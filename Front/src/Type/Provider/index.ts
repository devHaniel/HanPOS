export interface Provider {
  id: number
  nombre: string
  telefono: string | null
  rtn: string | null
}

export interface ProviderRequest {
  nombre: string
  telefono?: string | null
  rtn?: string | null
}