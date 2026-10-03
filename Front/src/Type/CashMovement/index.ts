import type { PageQuery } from '../Common'

export type CashMovementKind = 1 | 2

export interface CashMovement {
  id: number
  monto: number
  tipoMovimiento: CashMovementKind
  concepto: string | null
  fecha: string
  cajaId: number
}

export interface CashMovementRequest {
  monto: number
  tipoMovimiento: CashMovementKind
  concepto?: string | null
  fecha?: string
  cajaId: number
}

export interface CashMovementUpdateRequest extends CashMovementRequest {
  id: number
  fecha: string
}

export interface CashMovementQuery extends PageQuery {}