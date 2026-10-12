<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { DollarSign, CreditCard, Banknote, Package, Calculator, AlertTriangle, Wallet, CheckCircle, AlertCircle, ArrowRight, Store, ShoppingCart } from '@lucide/vue'
import { saleService, purchaseService, cashRegisterService } from '@/Service'
import { useProductStore } from '@/Store'
import { useCategoryStore } from '@/Store/CategoryStore'
import { useNegocioStore } from '@/Store'
import type { Sale } from '@/Type/Sale'
import type { Purchase } from '@/Type/Purchase'
import type { CashRegister } from '@/Type/CashRegister'

const products = useProductStore()
const categories = useCategoryStore()
const negocioStore = useNegocioStore()
const recentSales = ref<Sale[]>([])
const recentPurchases = ref<Purchase[]>([])
const currentRegister = ref<CashRegister | null>(null)
const loadError = ref('')

// Daily summary data
const todaySales = ref<Sale[]>([])
const todayPurchases = ref<Purchase[]>([])
const loading = ref(false)

const currency = (amount: number) => new Intl.NumberFormat('es-HN', { style: 'currency', currency: 'HNL' }).format(amount)
const shortDate = (value: string) => new Intl.DateTimeFormat('es-HN', { day: '2-digit', month: 'short' }).format(new Date(value))
const shortDateTime = (value: string) => new Intl.DateTimeFormat('es-HN', { day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit' }).format(new Date(value))

function paymentMethodLabel(method: number | null) {
  if (method === 1) return 'Efectivo'
  if (method === 2) return 'Tarjeta'
  if (method === 3) return 'Transferencia'
  return 'Sin definir'
}

function paymentMethodIcon(method: number | null) {
  if (method === 1) return '💵'
  if (method === 2) return '💳'
  if (method === 3) return '🏦'
  return '❓'
}

function isToday(dateString: string) {
  const date = new Date(dateString)
  const today = new Date()
  return date.getDate() === today.getDate() && 
         date.getMonth() === today.getMonth() && 
         date.getFullYear() === today.getFullYear()
}

function getTodayString(): string {
  return new Intl.DateTimeFormat('es-HN', { weekday: 'long', day: 'numeric', month: 'long' }).format(new Date())
}

const todaySalesByMethod = computed(() => {
  const result = { efectivo: 0, tarjeta: 0, transferencia: 0, total: 0, count: 0 }
  for (const sale of todaySales.value) {
    if (sale.estado === 2) continue // Skip cancelled
    result.total += sale.total
    result.count++
    if (sale.metodoPago === 1) result.efectivo += sale.total
    else if (sale.metodoPago === 2) result.tarjeta += sale.total
    else if (sale.metodoPago === 3) result.transferencia += sale.total
  }
  return result
})

const todayPurchasesTotal = computed(() => 
  todayPurchases.value.reduce((sum, p) => sum + p.total, 0)
)

const todayPurchasesCount = computed(() => 
  todayPurchases.value.length
)

const netCash = computed(() => todaySalesByMethod.value.efectivo - todayPurchasesTotal.value)

const isRegisterOpen = computed(() => currentRegister.value !== null)

// Setup checklist
const setupChecklist = computed(() => [
  { 
    id: 'negocio', 
    label: 'Datos del negocio', 
    done: !!negocioStore.negocio, 
    icon: Store,
    action: '/settings',
    actionLabel: 'Configurar'
  },
  { 
    id: 'categorias', 
    label: 'Categorías creadas', 
    done: categories.items.length > 0, 
    icon: Package,
    action: '/categories',
    actionLabel: 'Agregar'
  },
  { 
    id: 'productos', 
    label: 'Productos en catálogo', 
    done: products.items.length > 0, 
    icon: ShoppingCart,
    action: '/products',
    actionLabel: 'Agregar'
  },
  { 
    id: 'caja', 
    label: 'Caja abierta', 
    done: isRegisterOpen.value, 
    icon: Wallet,
    action: '/cash-register',
    actionLabel: 'Abrir caja'
  },
])

const setupComplete = computed(() => setupChecklist.value.every(item => item.done))
const setupProgress = computed(() => setupChecklist.value.filter(item => item.done).length)

async function load() {
  loading.value = true
  loadError.value = ''
  
  try {
    const today = new Date()
    const todayStr = today.toISOString().split('T')[0] ?? ''
    
    const results = await Promise.allSettled([
      products.fetchAll({ pagina: 1, cantidad: 10 }),
      categories.fetchAll({ pagina: 1, cantidad: 100 }),
      saleService.getByDate(todayStr),
      purchaseService.getByDate(todayStr),
      cashRegisterService.getOpen(),
      saleService.list({ pagina: 1, cantidad: 5 }),
      purchaseService.list({ pagina: 1, cantidad: 5 }),
    ])
    
    if (results[2].status === 'fulfilled') todaySales.value = results[2].value
    if (results[3].status === 'fulfilled') todayPurchases.value = results[3].value
    if (results[4].status === 'fulfilled') currentRegister.value = results[4].value
    if (results[5].status === 'fulfilled') recentSales.value = results[5].value.items
    if (results[6].status === 'fulfilled') recentPurchases.value = results[6].value.items
    
    if (results.some(r => r.status === 'rejected')) {
      loadError.value = 'Algunos datos no se pudieron cargar.'
    }
  } catch {
    loadError.value = 'Error al cargar el resumen diario.'
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <section class="fade-up">
    <div class="page-heading">
      <div>
        <p class="eyebrow">{{ getTodayString() }}</p>
        <h1>Resumen del día</h1>
        <p>Ventas, compras y efectivo neto de hoy.</p>
      </div>
      <RouterLink class="btn" to="/pos">Nueva venta <span aria-hidden="true">→</span></RouterLink>
    </div>

    <!-- DAILY SUMMARY CARDS -->
    <div class="metric-grid">
      <!-- Ventas de hoy - XL (span 2) -->
      <article class="metric-card metric-card-xl metric-card-sales">
        <div class="metric-top">
          <span>Ventas de hoy</span>
          <span class="metric-mark"><DollarSign :size="20"/></span>
        </div>
        <div class="metric-value metric-value-xl">{{ currency(todaySalesByMethod.total) }}</div>
        <div class="metric-note">{{ todaySalesByMethod.count }} ventas completadas</div>
      </article>
      
      <!-- Compras de hoy - XL (span 2) -->
      <article class="metric-card metric-card-xl metric-card-purchases">
        <div class="metric-top">
          <span>Compras de hoy</span>
          <span class="metric-mark"><Package :size="20"/></span>
        </div>
        <div class="metric-value metric-value-xl">{{ currency(todayPurchasesTotal) }}</div>
        <div class="metric-note">{{ todayPurchasesCount }} órdenes de compra</div>
      </article>

      <!-- Efectivo -->
      <article class="metric-card">
        <div class="metric-top">
          <span>Efectivo</span>
          <span class="metric-mark metric-mark-success"><Wallet :size="16"/></span>
        </div>
        <div class="metric-value metric-value-success">{{ currency(todaySalesByMethod.efectivo) }}</div>
        <div class="metric-note">En caja</div>
      </article>
      
      <!-- Tarjeta -->
      <article class="metric-card">
        <div class="metric-top">
          <span>Tarjeta</span>
          <span class="metric-mark metric-mark-info"><CreditCard :size="16"/></span>
        </div>
        <div class="metric-value metric-value-info">{{ currency(todaySalesByMethod.tarjeta) }}</div>
        <div class="metric-note">Depósito bancario</div>
      </article>
      
      <!-- Transferencia -->
      <article class="metric-card">
        <div class="metric-top">
          <span>Transferencia</span>
          <span class="metric-mark metric-mark-purple"><Banknote :size="16"/></span>
        </div>
        <div class="metric-value metric-value-purple">{{ currency(todaySalesByMethod.transferencia) }}</div>
        <div class="metric-note">Verificar en banco</div>
      </article>

      <!-- Efectivo neto en caja -->
      <article class="metric-card" :class="{ 'metric-card-positive': netCash >= 0, 'metric-card-negative': netCash < 0 }">
        <div class="metric-top">
          <span>Efectivo neto en caja</span>
          <span class="metric-mark" :class="netCash >= 0 ? 'metric-mark-success' : 'metric-mark-danger'">
            <Calculator :size="16"/>
          </span>
        </div>
        <div class="metric-value" :class="netCash >= 0 ? 'metric-value-success' : 'metric-value-danger'">{{ currency(netCash) }}</div>
        <div class="metric-note">Ventas efectivo - Compras</div>
      </article>

      <!-- Inventario bajo -->
      <article class="metric-card">
        <div class="metric-top">
          <span>Inventario bajo</span>
          <span class="metric-mark metric-mark-danger"><AlertTriangle :size="16"/></span>
        </div>
        <div class="metric-value">{{ products.items.filter((p) => p.stock <= p.stockMinimo).length }}</div>
        <div class="metric-note">Productos por debajo del mínimo</div>
      </article>
    </div>

    <!-- RECENT ACTIVITY -->
    <div class="dashboard-grid">
      <!-- Recent Sales -->
      <section class="panel">
        <div class="panel-header">
          <div><h2>Ventas recientes</h2><p>Últimas {{ recentSales.length }} ventas</p></div>
          <RouterLink class="table-action" to="/sales">Ver todas →</RouterLink>
        </div>
        <div v-if="loading" class="loading-state">Cargando…</div>
        <div v-else class="table-wrap">
          <table class="data-table">
            <thead><tr><th>Hora</th><th>Venta</th><th>Cliente</th><th>Pago</th><th>Total</th></tr></thead>
            <tbody>
              <tr v-for="sale in recentSales" :key="sale.id">
                <td>{{ shortDateTime(sale.fecha) }}</td>
                <td class="row-primary">#{{ sale.id }}</td>
                <td>{{ sale.clienteId ? `Cliente #${sale.clienteId}` : 'Consumidor final' }}</td>
                <td style="white-space: nowrap;">{{ paymentMethodIcon(sale.metodoPago) }} {{ paymentMethodLabel(sale.metodoPago) }}</td>
                <td class="row-primary">{{ currency(sale.total) }}</td>
              </tr>
            </tbody>
          </table>
          <div v-if="!recentSales.length && !loading" class="empty-state">No hay ventas recientes.</div>
        </div>
      </section>

      <!-- Recent Purchases -->
      <section class="panel">
        <div class="panel-header">
          <div><h2>Compras recientes</h2><p>Últimas {{ recentPurchases.length }} compras</p></div>
          <RouterLink class="table-action" to="/purchases">Ver todas →</RouterLink>
        </div>
        <div v-if="loading" class="loading-state">Cargando…</div>
        <div v-else class="table-wrap">
          <table class="data-table">
            <thead><tr><th>Fecha</th><th>Compra</th><th>Proveedor</th><th>Total</th></tr></thead>
            <tbody>
              <tr v-for="purchase in recentPurchases" :key="purchase.id">
                <td>{{ shortDate(purchase.fecha) }}</td>
                <td class="row-primary">#{{ purchase.id }}</td>
                <td>Proveedor #{{ purchase.proveedorId }}</td>
                <td class="row-primary">{{ currency(purchase.total) }}</td>
              </tr>
            </tbody>
          </table>
          <div v-if="!recentPurchases.length && !loading" class="empty-state">No hay compras recientes.</div>
        </div>
      </section>
    </div>
  </section>
</template>

<style scoped>
.setup-checklist-panel {
  margin-bottom: 24px;
}

.setup-checklist-header {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 16px;
  padding: 16px 20px;
  background: var(--color-accent-subtle);
  border: 1px solid var(--color-accent);
  border-radius: 12px;
}

.setup-progress {
  flex: 1;
  min-width: 0;
}

.setup-progress-bar {
  height: 8px;
  background: var(--color-surface);
  border-radius: 4px;
  overflow: hidden;
  margin-bottom: 6px;
}

.setup-progress-fill {
  height: 100%;
  background: var(--color-accent);
  border-radius: 4px;
  transition: width 0.3s ease;
}

.setup-progress-text {
  font-size: 12px;
  color: var(--color-muted);
  font-weight: 600;
}

.setup-checklist-header h2 {
  margin: 0;
  font-size: 16px;
  font-weight: 700;
  color: var(--color-fg);
  white-space: nowrap;
}

.setup-checklist-header p {
  margin: 4px 0 0;
  font-size: 12px;
  color: var(--color-muted);
}

.setup-checklist-items {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.setup-checklist-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 14px 16px;
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: 10px;
  transition: all 0.2s;
}

.setup-checklist-item:hover {
  border-color: var(--color-accent);
}

.setup-checklist-item.done {
  border-color: var(--color-success);
  background: var(--color-success-bg);
}

.item-icon {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  display: grid;
  place-items: center;
  flex-shrink: 0;
  background: var(--color-surface-hover);
  border: 1px solid var(--color-border);
  color: var(--color-muted);
  transition: all 0.2s;
}

.setup-checklist-item.done .item-icon {
  background: var(--color-success);
  border-color: var(--color-success);
  color: white;
}

.item-icon svg {
  stroke-width: 2;
}

.item-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 2px;
  min-width: 0;
}

.item-content strong {
  font-size: 14px;
  color: var(--color-fg);
}

.item-status {
  font-size: 11px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.3px;
}

.item-status.done {
  color: var(--color-success);
}

.item-status.pending {
  color: var(--color-warning);
}

.item-done-badge {
  flex-shrink: 0;
}

.btn-sm {
  min-height: 34px;
  padding: 6px 12px;
  font-size: 11px;
}

@media (max-width: 640px) {
  .setup-checklist-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }
  
  .setup-checklist-header h2 {
    white-space: normal;
  }
}
</style>
