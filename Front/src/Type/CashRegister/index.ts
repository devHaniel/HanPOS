import type { PageQuery } from '../Common'

export interface CashRegister {
  id: number
  fechaApertura: string
  fechaCierre: string | null
  montoInicial: number
  montoFinal: number | null
  estaAbierta: boolean
  usuarioId: number
}

export interface CashRegisterRequest {
  fechaApertura?: string
  montoInicial: number
  usuarioId: number
}

export interface CashRegisterUpdateRequest {
  id: number
  fechaApertura: string
  fechaCierre: string | null
  montoInicial: number
  montoFinal: number | null
  usuarioId: number
}

export interface CashRegisterQuery extends PageQuery {}