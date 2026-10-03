import { createEntityStore } from '../shared/createEntityStore'
import { purchaseService } from '@/Service'
import type { Purchase, PurchaseRequest, PurchaseUpdateRequest } from '@/Type/Purchase'

export const usePurchaseStore = createEntityStore<Purchase, PurchaseRequest, PurchaseUpdateRequest>('purchases', purchaseService)