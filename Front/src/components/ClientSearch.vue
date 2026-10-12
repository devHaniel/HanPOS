<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, nextTick } from 'vue'
import { Search, X, UserRound, ChevronDown, ChevronUp, UserPlus } from '@lucide/vue'
import type { Client } from '@/Type/Client'
import { clientService } from '@/Service'

interface Props {
  modelValue: number | null
  clients?: Client[]
  placeholder?: string
  disabled?: boolean
  debounceMs?: number
  pageSize?: number
  allowCreate?: boolean
}

interface Emits {
  (e: 'update:modelValue', value: number | null): void
  (e: 'select', client: Client): void
  (e: 'clear'): void
  (e: 'create-client'): void
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: 'Buscar cliente por nombre, teléfono o RTN...',
  clients: () => [] as Client[],
  disabled: false,
  debounceMs: 250,
  pageSize: 15,
  allowCreate: true
})

const emit = defineEmits<Emits>()

const searchTerm = ref('')
const results = ref<Client[]>([])
const isOpen = ref(false)
const isLoading = ref(false)
const error = ref<string | null>(null)
const selectedIndex = ref(-1)
const totalResults = ref(0)
const currentPage = ref(1)
const hasMore = ref(false)
const searchAbortController = ref<AbortController | null>(null)
const showCreateHint = ref(false)

const inputRef = ref<HTMLInputElement | null>(null)
const dropdownRef = ref<HTMLDivElement | null>(null)

const displayValue = computed(() => {
  if (!props.modelValue) return ''
  // Check results first, then pre-loaded clients
  let client = results.value.find(c => c.id === props.modelValue)
  if (!client) {
    client = props.clients?.find(c => c.id === props.modelValue)
  }
  return client ? `${client.nombre}${client.telefono ? ` · ${client.telefono}` : ''}${client.rtn ? ` · ${client.rtn}` : ''}` : ''
})

async function performSearch(page = 1, append = false) {
  if (searchAbortController.value) {
    searchAbortController.value.abort()
  }
  searchAbortController.value = new AbortController()

  isLoading.value = true
  error.value = null

  try {
    const response = await clientService.search(
      searchTerm.value.trim() || undefined,
      page,
      props.pageSize
    )

    if (append) {
      results.value = [...results.value, ...response.items]
    } else {
      results.value = response.items
    }
    totalResults.value = response.total
    currentPage.value = page
    hasMore.value = response.items.length === props.pageSize && results.value.length < response.total
    selectedIndex.value = -1
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Error al buscar clientes'
    if (!append) results.value = []
  } finally {
    isLoading.value = false
  }
}

const debounceTimer = ref<ReturnType<typeof setTimeout> | null>(null)

function debouncedSearch() {
  if (debounceTimer.value) clearTimeout(debounceTimer.value)
  debounceTimer.value = window.setTimeout(() => {
    currentPage.value = 1
    performSearch(1, false)
  }, props.debounceMs)
}

function handleInputChange() {
  debouncedSearch()
  if (!isOpen.value) openDropdown()
}

function openDropdown() {
  if (props.disabled) return
  isOpen.value = true
  if (searchTerm.value.trim() && results.value.length === 0) {
    performSearch(1, false)
  }
  // Show create hint after a brief moment if no results
  showCreateHint.value = false
  setTimeout(() => {
    if (isOpen.value && searchTerm.value.trim() && results.value.length === 0) {
      showCreateHint.value = true
    }
  }, 300)
  nextTick(() => inputRef.value?.focus())
}

function closeDropdown() {
  isOpen.value = false
  searchTerm.value = ''
  results.value = []
  selectedIndex.value = -1
  showCreateHint.value = false
}

function handleKeydown(e: KeyboardEvent) {
  if (!isOpen.value) {
    if (e.key === 'ArrowDown' || e.key === 'Enter' || e.key === ' ') {
      e.preventDefault()
      openDropdown()
    }
    return
  }

  switch (e.key) {
    case 'ArrowDown':
      e.preventDefault()
      const maxIndex = results.value.length + (props.allowCreate && showCreateHint.value ? 1 : 0) - 1
      selectedIndex.value = Math.min(selectedIndex.value + 1, maxIndex)
      scrollToSelected()
      break
    case 'ArrowUp':
      e.preventDefault()
      selectedIndex.value = Math.max(selectedIndex.value - 1, -1)
      scrollToSelected()
      break
    case 'Enter':
      e.preventDefault()
      if (selectedIndex.value >= 0 && selectedIndex.value < results.value.length) {
        const client = results.value[selectedIndex.value]
        if (client) selectClient(client)
      } else if (props.allowCreate && showCreateHint.value && selectedIndex.value === results.value.length) {
        createNewClient()
      }
      break
    case 'Escape':
      closeDropdown()
      break
  }
}

function selectClient(client: Client) {
  emit('update:modelValue', client.id)
  emit('select', client)
  closeDropdown()
}

function clearSelection() {
  emit('update:modelValue', null)
  emit('clear')
}

function createNewClient() {
  closeDropdown()
  emit('create-client')
}

function scrollToSelected() {
  const items = dropdownRef.value?.querySelectorAll('.search-result-item, .create-client-hint')
  const selected = items?.[selectedIndex.value] as HTMLElement
  if (selected) {
    selected.scrollIntoView({ block: 'nearest' })
  }
}

function loadMore() {
  if (!hasMore.value || isLoading.value) return
  performSearch(currentPage.value + 1, true)
}

function handleClickOutside(e: MouseEvent) {
  if (dropdownRef.value && !dropdownRef.value.contains(e.target as Node) &&
      inputRef.value && !inputRef.value.contains(e.target as Node)) {
    closeDropdown()
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
  if (debounceTimer.value) clearTimeout(debounceTimer.value)
  if (searchAbortController.value) searchAbortController.value.abort()
})
</script>

<template>
  <div class="client-search" :class="{ open: isOpen, disabled: props.disabled }">
    <div class="search-input-wrapper" @click="openDropdown">
      <Search :size="16" class="search-icon" />
      <input
        ref="inputRef"
        type="text"
        :placeholder="props.placeholder"
        :disabled="props.disabled"
        :value="displayValue"
        @input="handleInputChange"
        @keydown="handleKeydown"
        @focus="openDropdown"
        class="search-input"
        autocomplete="off"
        aria-autocomplete="list"
        :aria-expanded="isOpen"
        aria-controls="client-search-results"
      />
      <div class="search-actions">
        <button
          v-if="props.modelValue"
          type="button"
          class="clear-button"
          @click.stop="clearSelection"
          aria-label="Limpiar selección"
        >
          <X :size="14" />
        </button>
        <component :is="isOpen ? ChevronUp : ChevronDown" :size="16" class="dropdown-icon" />
      </div>
    </div>

    <Transition name="dropdown">
      <div
        v-show="isOpen"
        ref="dropdownRef"
        id="client-search-results"
        class="search-dropdown"
        role="listbox"
        aria-label="Resultados de búsqueda de clientes"
      >
        <div v-if="isLoading && results.length === 0" class="search-loading">
          <div class="spinner" />
          <span>Buscando clientes...</span>
        </div>

        <div v-else-if="error" class="search-error">{{ error }}</div>

        <div v-else class="search-results-list">
          <div
            v-for="(client, index) in results"
            :key="client.id"
            class="search-result-item"
            :class="{ selected: index === selectedIndex, 'is-selected': client.id === props.modelValue }"
            role="option"
            :aria-selected="client.id === props.modelValue"
            @click="selectClient(client)"
            @mousemove="selectedIndex = index"
          >
            <div class="client-info">
              <span class="client-name">{{ client.nombre }}</span>
              <span class="client-meta">
                <span v-if="client.telefono"><UserRound :size="10" /> {{ client.telefono }}</span>
                <span v-if="client.rtn">📋 {{ client.rtn }}</span>
              </span>
            </div>
          </div>

          <!-- "Nuevo cliente rápido" hint -->
          <div 
            v-if="props.allowCreate && showCreateHint" 
            class="create-client-hint"
            :class="{ selected: selectedIndex === results.length }"
            role="option"
            @click="createNewClient"
            @mousemove="selectedIndex = results.length"
          >
            <div class="create-client-content">
              <UserPlus :size="16" />
              <span>Crear nuevo cliente: "{{ searchTerm }}"</span>
            </div>
          </div>

          <div v-if="hasMore" class="load-more" @click="loadMore">
            <span v-if="isLoading">Cargando más...</span>
            <span v-else>Cargar más resultados ({{ results.length }} de {{ totalResults }})</span>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
.client-search {
  position: relative;
  width: 100%;
}

.client-search.disabled .search-input-wrapper {
  opacity: 0.6;
  cursor: not-allowed;
}

.search-input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
  gap: 8px;
  min-height: 40px;
  padding: 0 12px;
  border: 1px solid #dce2da;
  border-radius: 7px;
  background: #fff;
  color: var(--ink);
  font-size: 12px;
  transition: border-color 0.2s, box-shadow 0.2s;
  cursor: text;
}

.search-input-wrapper:hover:not(.disabled) {
  border-color: #b4cf91;
}

.search-input-wrapper:focus-within {
  outline: none;
  border-color: var(--accent);
  box-shadow: 0 0 0 3px var(--accent-focus);
}

.search-icon {
  color: var(--muted);
  flex-shrink: 0;
}

.search-input {
  flex: 1;
  border: none;
  background: transparent;
  color: var(--ink);
  font-size: 12px;
  outline: none;
  min-width: 0;
}

.search-input::placeholder {
  color: #a0aaa1;
}

.search-input:disabled {
  color: var(--muted);
}

.search-actions {
  display: flex;
  align-items: center;
  gap: 4px;
}

.clear-button {
  display: grid;
  width: 28px;
  height: 28px;
  place-items: center;
  border: none;
  border-radius: 5px;
  background: transparent;
  color: var(--muted);
  cursor: pointer;
  transition: background 0.2s, color 0.2s;
}

.clear-button:hover {
  background: var(--surface-hover);
  color: var(--danger);
}

.dropdown-icon {
  color: var(--muted);
  flex-shrink: 0;
  transition: transform 0.2s;
}

.search-dropdown {
  position: absolute;
  top: calc(100% + 4px);
  left: 0;
  right: 0;
  max-height: 400px;
  border: 1px solid var(--line);
  border-radius: 8px;
  background: white;
  box-shadow: 0 8px 24px rgba(0,0,0,0.12);
  z-index: 50;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.dropdown-enter-active,
.dropdown-leave-active {
  transition: opacity 0.15s ease, transform 0.15s ease;
}

.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}

.search-loading,
.search-empty,
.search-error {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  padding: 24px 16px;
  color: var(--muted);
  font-size: 12px;
  text-align: center;
}

.search-error {
  color: var(--danger);
}

.spinner {
  width: 20px;
  height: 20px;
  border: 2px solid var(--line);
  border-top-color: var(--accent);
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.search-results-list {
  flex: 1;
  overflow-y: auto;
  padding: 4px;
}

.search-result-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  padding: 10px 12px;
  border-radius: 6px;
  cursor: pointer;
  transition: background 0.1s;
}

.search-result-item:hover,
.search-result-item.selected {
  background: var(--surface-hover);
}

.search-result-item.is-selected {
  background: var(--accent-subtle);
}

.search-result-item.is-selected:hover {
  background: var(--accent-light);
}

.client-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
  min-width: 0;
  flex: 1;
}

.client-name {
  font-size: 13px;
  color: var(--fg);
  font-weight: 500;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.client-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  font-size: 10px;
  color: var(--muted);
}

.client-meta span {
  display: flex;
  align-items: center;
  gap: 4px;
}

/* Create client hint */
.create-client-hint {
  padding: 10px 12px;
  border-radius: 6px;
  cursor: pointer;
  transition: background 0.1s;
  border: 1px dashed var(--accent);
  background: var(--accent-subtle);
  margin: 0 4px 4px;
}

.create-client-hint:hover,
.create-client-hint.selected {
  background: var(--accent);
  border-style: solid;
}

.create-client-hint.selected {
  color: white;
}

.create-client-content {
  display: flex;
  align-items: center;
  gap: 8px;
  color: var(--accent);
  font-size: 12px;
  font-weight: 600;
}

.create-client-hint.selected .create-client-content {
  color: white;
}

.load-more {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 12px;
  border-top: 1px solid var(--line);
  margin-top: 4px;
  color: var(--muted);
  font-size: 11px;
  cursor: pointer;
  transition: background 0.2s;
}

.load-more:hover {
  background: var(--surface-hover);
  color: var(--fg);
}

@media (max-width: 480px) {
  .search-dropdown {
    left: -16px;
    right: -16px;
    border-radius: 0 0 12px 12px;
    max-height: 50vh;
  }
}
</style>