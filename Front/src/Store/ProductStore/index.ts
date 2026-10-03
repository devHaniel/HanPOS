import { createEntityStore } from '../shared/createEntityStore'
import { productService } from '@/Service'
import type { Product, ProductRequest } from '@/Type/Product'

export const useProductStore = createEntityStore<Product, ProductRequest>('products', productService)