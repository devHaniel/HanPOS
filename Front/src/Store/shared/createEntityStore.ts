import { computed, ref, shallowRef } from 'vue'
import { defineStore } from 'pinia'
import type { ListResult, PageQuery, PagedResult } from '@/Type/Common'
import type { CrudService } from '@/Service/CrudService'

const emptyPage = <T>(): PagedResult<T> => ({ items: [], pagina: 1, cantidad: 10, total: 0, totalPaginas: 0 })

export function createEntityStore<T extends { id: number }, TCreate, TUpdate = TCreate>(
  storeId: string,
  service: CrudService<T, TCreate, TUpdate>,
) {
  return defineStore(storeId, () => {
    const items = shallowRef<T[]>([])
    const selected = shallowRef<T | null>(null)
    const page = ref<PagedResult<T>>(emptyPage<T>())
    const isLoading = ref(false)
    const error = ref<string | null>(null)
    const total = computed(() => page.value.total)

    async function fetchAll(query: PageQuery = {}): Promise<ListResult<T>> {
      isLoading.value = true
      error.value = null
      try {
        const result = await service.list(query)
        if (Array.isArray(result)) {
          items.value = result
          page.value = { items: result, pagina: 1, cantidad: result.length, total: result.length, totalPaginas: 1 }
        } else {
          page.value = result
          items.value = result.items
        }
        return result
      } catch (cause) {
        error.value = cause instanceof Error ? cause.message : 'No se pudo cargar la información'
        throw cause
      } finally {
        isLoading.value = false
      }
    }

    async function fetchById(id: number): Promise<T> {
      isLoading.value = true
      error.value = null
      try {
        selected.value = await service.getById(id)
        return selected.value
      } catch (cause) {
        error.value = cause instanceof Error ? cause.message : 'No se pudo cargar el registro'
        throw cause
      } finally {
        isLoading.value = false
      }
    }

    async function create(request: TCreate): Promise<T> {
      const created = await service.create(request)
      items.value = [created, ...items.value]
      page.value = { ...page.value, items: items.value, total: page.value.total + 1 }
      return created
    }

    async function update(id: number, request: TUpdate): Promise<T> {
      const updated = await service.update(id, request)
      items.value = items.value.map((item) => item.id === id ? updated : item)
      if (selected.value?.id === id) selected.value = updated
      page.value = { ...page.value, items: items.value }
      return updated
    }

    async function remove(id: number): Promise<void> {
      await service.remove(id)
      items.value = items.value.filter((item) => item.id !== id)
      if (selected.value?.id === id) selected.value = null
      page.value = { ...page.value, items: items.value, total: Math.max(0, page.value.total - 1) }
    }

    function clearError(): void {
      error.value = null
    }

    return { items, selected, page, total, isLoading, error, fetchAll, fetchById, create, update, remove, clearError }
  })
}