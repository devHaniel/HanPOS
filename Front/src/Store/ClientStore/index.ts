import { createEntityStore } from '../shared/createEntityStore'
import { clientService } from '@/Service'
import type { Client, ClientRequest } from '@/Type/Client'

export const useClientStore = createEntityStore<Client, ClientRequest>('clients', clientService)