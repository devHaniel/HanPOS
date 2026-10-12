<script setup lang="ts">
import { computed } from 'vue'
import { CheckCircle, AlertCircle, Info, AlertTriangle, Loader2, X } from '@lucide/vue'
import { useToast } from '@/composables/useToast'

const { toasts, remove } = useToast()

const icons = {
  success: CheckCircle,
  error: AlertCircle,
  info: Info,
  warning: AlertTriangle,
  loading: Loader2
}

const colors = {
  success: 'var(--color-success)',
  error: 'var(--color-danger)',
  info: 'var(--color-info)',
  warning: 'var(--color-warning)',
  loading: 'var(--color-accent)'
}

const bgColors = {
  success: 'var(--color-success-bg)',
  error: 'var(--color-danger-bg)',
  info: 'var(--color-info-bg)',
  warning: 'var(--color-warning-bg)',
  loading: 'var(--color-accent-subtle)'
}

const borderColors = {
  success: 'var(--color-success)',
  error: 'var(--color-danger)',
  info: 'var(--color-info)',
  warning: 'var(--color-warning)',
  loading: 'var(--color-accent)'
}
</script>

<template>
  <div class="toast-container" aria-live="polite" aria-atomic="true">
    <div 
      v-for="toast in toasts" 
      :key="toast.id" 
      class="toast"
      :class="toast.type"
      :style="{ borderLeftColor: borderColors[toast.type], background: bgColors[toast.type] }"
      role="alert"
    >
      <div class="toast-icon" :style="{ color: colors[toast.type] }">
        <component :is="icons[toast.type]" :size="20" :stroke-width="toast.type === 'success' || toast.type === 'error' ? 2.5 : 2" />
      </div>
      <div class="toast-content">
        <strong>{{ toast.title }}</strong>
        <span v-if="toast.message">{{ toast.message }}</span>
      </div>
      <button 
        class="toast-close" 
        @click="remove(toast.id)" 
        :aria-label="'Cerrar notificacion'"
        :style="{ color: colors[toast.type] }"
      >
        <X :size="16" />
      </button>
      <div v-if="toast.type === 'loading'" class="toast-spinner" :style="{ borderTopColor: colors[toast.type] }" />
    </div>
  </div>
</template>

<style scoped>
.toast-container {
  position: fixed;
  top: 80px;
  right: 24px;
  z-index: 1000;
  display: flex;
  flex-direction: column;
  gap: 8px;
  max-width: 380px;
  pointer-events: none;
}

.toast {
  pointer-events: auto;
  display: flex;
  align-items: flex-start;
  gap: 12px;
  padding: 14px 16px;
  border-radius: 10px;
  border-left: 4px solid;
  box-shadow: 0 8px 24px rgba(0,0,0,0.12);
  animation: slideInRight 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
  min-width: 280px;
  max-width: 380px;
}

@keyframes slideInRight {
  from { opacity: 0; transform: translateX(100%); }
  to { opacity: 1; transform: translateX(0); }
}

.toast-icon {
  flex-shrink: 0;
  margin-top: 2px;
}

.toast-content {
  flex: 1;
  min-width: 0;
}

.toast-content strong {
  display: block;
  font-size: 13px;
  color: var(--color-fg);
  margin-bottom: 2px;
}

.toast-content span {
  display: block;
  font-size: 12px;
  color: var(--color-muted);
  line-height: 1.4;
}

.toast-close {
  flex-shrink: 0;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  border-radius: 4px;
  background: transparent;
  cursor: pointer;
  opacity: 0.6;
  transition: opacity 0.2s, background 0.2s;
}

.toast-close:hover {
  opacity: 1;
  background: var(--color-surface-hover);
}

.toast-spinner {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 3px;
  border-radius: 0 0 10px 10px;
  border-top: 2px solid transparent;
  border-top-width: 2px;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

@media (max-width: 640px) {
  .toast-container {
    left: 12px;
    right: 12px;
    top: auto;
    bottom: 12px;
    max-width: none;
  }
  
  .toast {
    min-width: auto;
    max-width: none;
    animation: slideUp 0.3s ease;
  }
  
  @keyframes slideUp {
    from { opacity: 0; transform: translateY(20px); }
    to { opacity: 1; transform: translateY(0); }
  }
}
</style>