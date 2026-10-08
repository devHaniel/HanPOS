import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { negocioService } from '@/Service'
import type { Negocio, NegocioRequest } from '@/Type/Negocio'

export const useNegocioStore = defineStore('negocio', () => {
  const negocio = ref<Negocio | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  async function fetch(): Promise<Negocio | null> {
    isLoading.value = true
    error.value = null
    try {
      negocio.value = await negocioService.get()
      return negocio.value
    } catch (cause) {
      error.value = cause instanceof Error ? cause.message : 'No se pudo cargar la configuración del negocio'
      throw cause
    } finally {
      isLoading.value = false
    }
  }

  async function create(request: NegocioRequest): Promise<Negocio> {
    isLoading.value = true
    error.value = null
    try {
      const created = await negocioService.create(request)
      negocio.value = created
      return created
    } catch (cause) {
      error.value = cause instanceof Error ? cause.message : 'No se pudo crear la configuración del negocio'
      throw cause
    } finally {
      isLoading.value = false
    }
  }

  async function update(request: NegocioRequest): Promise<Negocio> {
    if (!negocio.value) throw new Error('No hay negocio configurado')
    
    isLoading.value = true
    error.value = null
    try {
      const updated = await negocioService.update(negocio.value.id, request)
      negocio.value = updated
      return updated
    } catch (cause) {
      error.value = cause instanceof Error ? cause.message : 'No se pudo actualizar la configuración del negocio'
      throw cause
    } finally {
      isLoading.value = false
    }
  }

  async function checkExists(): Promise<boolean> {
    try {
      return await negocioService.exists()
    } catch {
      return false
    }
  }

  function clearError(): void {
    error.value = null
  }

  // Computed para acceso fácil a preferencias parseadas
  const preferencias = computed(() => {
    if (!negocio.value?.preferenciasPantalla) return {}
    try {
      return JSON.parse(negocio.value.preferenciasPantalla)
    } catch {
      return {}
    }
  })

  const configuracionImpresion = computed(() => {
    if (!negocio.value?.configuracionImpresion) return {}
    try {
      return JSON.parse(negocio.value.configuracionImpresion)
    } catch {
      return {}
    }
  })

  return {
    negocio,
    isLoading,
    error,
    preferencias,
    configuracionImpresion,
    fetch,
    create,
    update,
    checkExists,
    clearError
  }
})