import { createEntityStore } from '../shared/createEntityStore'
import { providerService } from '@/Service'
import type { Provider, ProviderRequest } from '@/Type/Provider'

export const useProviderStore = createEntityStore<Provider, ProviderRequest>('providers', providerService)