import { createEntityStore } from '../shared/createEntityStore'
import { cashMovementService } from '@/Service'
import type { CashMovement, CashMovementRequest, CashMovementUpdateRequest } from '@/Type/CashMovement'

export const useCashMovementStore = createEntityStore<CashMovement, CashMovementRequest, CashMovementUpdateRequest>('cash-movements', cashMovementService)