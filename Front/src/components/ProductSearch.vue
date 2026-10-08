<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, nextTick } from 'vue'
import { Search, X, Package, ChevronDown, ChevronUp } from '@lucide/vue'
import type { Product } from '@/Type/Product'
import { productService } from '@/Service'

interface Props {
  modelValue: number | null
  placeholder?: string
  showStock?: boolean
  showPrice?: 'compra' | 'venta' | 'both' | 'none'
  disabled?: boolean
  debounceMs?: number
  pageSize?: number
}

interface Emits {
  (e: 'update:modelValue', value: number | null): void
  (e: 'select', product: Product): void
  (e: 'clear'): void
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: 'Buscar producto por nombre o código...',
  showStock: true,
  showPrice: 'compra',
  disabled: false,
  debounceMs: 300,
  pageSize: 20
})

const emit = defineEmits<Emits>()

const searchTerm = ref('')
const results = ref<Product[]>([])
const isOpen = ref(false)
const isLoading = ref(false)
const error = ref<string | null>(null)
const selectedIndex = ref(-1)
const totalResults = ref(0)
const currentPage = ref(1)
const hasMore = ref(false)
const searchAbortController = ref<AbortController | null>(null)

const inputRef = ref<HTMLInputElement | null>(null)
const dropdownRef = ref<HTMLDivElement | null>(null)

const displayValue = computed(() => {
  if (!props.modelValue) return ''
  const product = results.value.find(p => p.id === props.modelValue)
  return product ? `${product.codigo} · ${product.nombre}` : ''
})

function formatCurrency(amount: number, currency: string) {
  return new Intl.NumberFormat('es-HN', { style: 'currency', currency }).format(amount)
}

async function performSearch(page = 1, append = false) {
  if (searchAbortController.value) {
    searchAbortController.value.abort()
  }
  searchAbortController.value = new AbortController()

  isLoading.value = true
  error.value = null

  try {
    const response = await productService.search({
      termino: searchTerm.value.trim() || undefined,
      pagina: page,
      cantidad: props.pageSize
    })

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
    error.value = err instanceof Error ? err.message : 'Error al buscar productos'
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
  nextTick(() => inputRef.value?.focus())
}

function closeDropdown() {
  isOpen.value = false
  searchTerm.value = ''
  results.value = []
  selectedIndex.value = -1
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
      selectedIndex.value = Math.min(selectedIndex.value + 1, results.value.length - 1)
      scrollToSelected()
      break
    case 'ArrowUp':
      e.preventDefault()
      selectedIndex.value = Math.max(selectedIndex.value - 1, -1)
      scrollToSelected()
      break
    case 'Enter':
      e.preventDefault()
      const product = results.value[selectedIndex.value]
      if (selectedIndex.value >= 0 && product) {
        selectProduct(product)
      }
      break
    case 'Escape':
      closeDropdown()
      break
  }
}

function selectProduct(product: Product) {
  emit('update:modelValue', product.id)
  emit('select', product)
  closeDropdown()
}

function clearSelection() {
  emit('update:modelValue', null)
  emit('clear')
}

function scrollToSelected() {
  const items = dropdownRef.value?.querySelectorAll('.search-result-item')
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
  <div class="product-search" :class="{ open: isOpen, disabled: props.disabled }">
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
        aria-controls="search-results"
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
        id="search-results"
        class="search-dropdown"
        role="listbox"
        aria-label="Resultados de búsqueda"
      >
        <div v-if="isLoading && results.length === 0" class="search-loading">
          <div class="spinner" />
          <span>Buscando productos...</span>
        </div>

        <div v-else-if="error" class="search-error">{{ error }}</div>

        <div v-else-if="results.length === 0 && searchTerm" class="search-empty">
          <Package :size="24" />
          <p>No se encontraron productos</p>
          <small>Intenta con otro término de búsqueda</small>
        </div>

        <div v-else-if="results.length === 0 && !searchTerm" class="search-empty">
          <Package :size="24" />
          <p>Escribe para buscar productos</p>
          <small>Busca por nombre o código</small>
        </div>

        <div v-else class="search-results-list">
          <div
            v-for="(product, index) in results"
            :key="product.id"
            class="search-result-item"
            :class="{ selected: index === selectedIndex, 'is-selected': product.id === props.modelValue }"
            role="option"
            :aria-selected="product.id === props.modelValue"
            @click="selectProduct(product)"
            @mousemove="selectedIndex = index"
          >
            <div class="product-info">
              <span class="product-code">{{ product.codigo }}</span>
              <span class="product-name">{{ product.nombre }}</span>
            </div>
            <div class="product-meta">
              <span v-if="props.showStock" class="stock-badge" :class="{ low: product.stock <= product.stockMinimo, empty: product.stock <= 0 }">
                Stock: {{ product.stock }}
              </span>
              <span v-if="props.showPrice === 'compra' || props.showPrice === 'both'" class="price">
                Costo: {{ formatCurrency(product.precioCompra, 'HNL') }}
              </span>
              <span v-if="props.showPrice === 'venta' || props.showPrice === 'both'" class="price sale">
                Venta: {{ formatCurrency(product.precioVenta, 'HNL') }}
              </span>
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
.product-search {
  position: relative;
  width: 100%;
}

.product-search.disabled .search-input-wrapper {
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

.search-empty p {
  margin: 0;
  color: var(--fg);
  font-weight: 500;
}

.search-empty small {
  margin: 0;
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

.product-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
  min-width: 0;
  flex: 1;
}

.product-code {
  font-size: 10px;
  font-weight: 700;
  color: var(--accent);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.product-name {
  font-size: 12px;
  color: var(--fg);
  font-weight: 500;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.product-meta {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 10px;
  flex-shrink: 0;
}

.stock-badge {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 2px 8px;
  border-radius: 20px;
  font-size: 10px;
  font-weight: 700;
  background: #eff5e8;
  color: #55723e;
}

.stock-badge.low {
  background: #fff7ed;
  color: #ea580c;
}

.stock-badge.empty {
  background: #fef2f2;
  color: #dc2626;
}

.price {
  font-size: 11px;
  font-weight: 600;
  color: #56635a;
}

.price.sale {
  color: var(--accent);
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