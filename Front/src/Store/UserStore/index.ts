import { createEntityStore } from '../shared/createEntityStore'
import { userService } from '@/Service'
import type { ManagedUser, UserCreateRequest, UserUpdateRequest } from '@/Type/User'

export const useUserStore = createEntityStore<ManagedUser, UserCreateRequest, UserUpdateRequest>('users', userService)