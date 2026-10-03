import { createEntityStore } from '../shared/createEntityStore'
import { cashRegisterService } from '@/Service'
import type { CashRegister, CashRegisterRequest, CashRegisterUpdateRequest } from '@/Type/CashRegister'

export const useCashRegisterStore = createEntityStore<CashRegister, CashRegisterRequest, CashRegisterUpdateRequest>('cash-registers', cashRegisterService)