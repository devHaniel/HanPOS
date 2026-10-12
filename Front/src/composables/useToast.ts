import { ref, shallowRef } from 'vue'
import { CheckCircle, AlertCircle, Info, Loader2 } from '@lucide/vue'

interface Toast {
  id: number
  type: 'success' | 'error' | 'info' | 'warning' | 'loading'
  title: string
  message?: string
  duration?: number
  persistent?: boolean
}

const toasts = shallowRef<Toast[]>([])
let toastId = 0

export function useToast() {
  function show(type: Toast['type'], title: string, message?: string, options?: { duration?: number; persistent?: boolean }) {
    const id = ++toastId
    const toast: Toast = {
      id,
      type,
      title,
      message,
      duration: options?.duration ?? (type === 'loading' ? 0 : 4000),
      persistent: options?.persistent ?? false
    }
    toasts.value.push(toast)
    
    if ((toast.duration ?? 0) > 0 && !toast.persistent) {
      setTimeout(() => remove(id), toast.duration)
    }
    return id
  }

  function remove(id: number) {
    const index = toasts.value.findIndex(t => t.id === id)
    if (index > -1) toasts.value.splice(index, 1)
  }

  function clear() {
    toasts.value = []
  }

  const success = (title: string, message?: string, options?: { duration?: number }) => 
    show('success', title, message, options)
  const error = (title: string, message?: string, options?: { duration?: number }) => 
    show('error', title, message, { ...options, duration: options?.duration ?? 6000 })
  const info = (title: string, message?: string, options?: { duration?: number }) => 
    show('info', title, message, options)
  const warning = (title: string, message?: string, options?: { duration?: number }) => 
    show('warning', title, message, options)
  const loading = (title: string, message?: string) => 
    show('loading', title, message, { persistent: true })

  return {
    toasts,
    show,
    remove,
    clear,
    success,
    error,
    info,
    warning,
    loading
  }
}

// Global instance for use outside components
let globalToast: ReturnType<typeof useToast> | null = null

export function getToast() {
  if (!globalToast) globalToast = useToast()
  return globalToast
}

export function useGlobalToast() {
  return getToast()
}