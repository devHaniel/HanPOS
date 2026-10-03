import { createEntityStore } from '../shared/createEntityStore'
import { saleService } from '@/Service'
import type { Sale, SaleRequest, SaleUpdateRequest } from '@/Type/Sale'

export const useSaleStore = createEntityStore<Sale, SaleRequest, SaleUpdateRequest>('sales', saleService)