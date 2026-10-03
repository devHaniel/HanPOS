export interface Client {
  id: number
  nombre: string
  telefono: string | null
  rtn: string | null
}

export interface ClientRequest {
  nombre: string
  telefono?: string | null
  rtn?: string | null
}