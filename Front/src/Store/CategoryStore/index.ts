import { createEntityStore } from '../shared/createEntityStore'
import { categoryService } from '@/Service'
import type { Category, CategoryRequest } from '@/Type/Category'

export const useCategoryStore = createEntityStore<Category, CategoryRequest>('categories', categoryService)