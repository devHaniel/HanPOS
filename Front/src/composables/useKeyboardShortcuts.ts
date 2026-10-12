import { onMounted, onUnmounted } from 'vue'

interface Shortcut {
  key: string
  ctrl?: boolean
  alt?: boolean
  shift?: boolean
  meta?: boolean
  action: () => void
  description: string
  preventDefault?: boolean
}

export function useKeyboardShortcuts(shortcuts: Shortcut[]) {
  function handleKeyDown(event: KeyboardEvent) {
    for (const shortcut of shortcuts) {
      const keyMatch = event.key.toLowerCase() === shortcut.key.toLowerCase()
      const ctrlMatch = !!shortcut.ctrl === event.ctrlKey
      const altMatch = !!shortcut.alt === event.altKey
      const shiftMatch = !!shortcut.shift === event.shiftKey
      const metaMatch = !!shortcut.meta === event.metaKey
      
      if (keyMatch && ctrlMatch && altMatch && shiftMatch && metaMatch) {
        if (shortcut.preventDefault !== false) event.preventDefault()
        shortcut.action()
        break
      }
    }
  }

  onMounted(() => window.addEventListener('keydown', handleKeyDown))
  onUnmounted(() => window.removeEventListener('keydown', handleKeyDown))
}

// Predefined shortcuts for common POS actions
export function createPosShortcuts(actions: {
  newSale?: () => void
  searchProducts?: () => void
  searchClients?: () => void
  completeSale?: () => void
  printReceipt?: () => void
  newPurchase?: () => void
  showHelp?: () => void
}) {
  const entries: [string, (() => void) | undefined, string, { ctrl?: boolean }?][] = [
    ['F2', actions.newSale, 'Nueva venta', {}],
    ['F5', actions.searchProducts, 'Buscar productos', {}],
    ['F6', actions.searchClients, 'Buscar clientes', {}],
    ['Enter', actions.completeSale, 'Completar venta (Ctrl+Enter)', { ctrl: true }],
    ['p', actions.printReceipt, 'Imprimir ticket (Ctrl+P)', { ctrl: true }],
    ['n', actions.newPurchase, 'Nueva compra (Ctrl+N)', { ctrl: true }],
    ['F1', actions.showHelp, 'Ayuda', {}],
  ]

  return entries
    .filter(([, action]) => !!action)
    .map(([key, action, description, mods]) => ({
      key,
      action: action!,
      description,
      ctrl: mods?.ctrl,
    }))
}