<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { ArrowRight, ClipboardList, Minus, Package, Plus, RefreshCw, Trash2 } from '@lucide/vue'
import { cashRegisterService, productService, providerService, purchaseService } from '@/Service'
import { useAuthStore } from '@/Store'
import ProductSearch from '@/components/ProductSearch.vue'
import type { CashRegister } from '@/Type/CashRegister'
import type { PagedResult } from '@/Type/Common'
import type { Product } from '@/Type/Product'
import type { Purchase } from '@/Type/Purchase'
import type { Provider } from '@/Type/Provider'

interface PurchaseLine { product: Product; quantity: number }

const auth = useAuthStore()
const result = ref<PagedResult<Purchase> | null>(null)
const providers = ref<Provider[]>([])
const register = ref<CashRegister | null>(null)
const providerId = ref<number | null>(null)
const productId = ref<number | null>(null)
const quantity = ref(1)
const lines = ref<PurchaseLine[]>([])
const loading = ref(false)
const saving = ref(false)
const error = ref('')
const notice = ref('')

const subtotal = computed(() => lines.value.reduce((sum, line) => sum + line.product.precioCompra * line.quantity, 0))
const tax = computed(() => subtotal.value * 0.15)
const total = computed(() => subtotal.value + tax.value)
const currency = (amount: number) => new Intl.NumberFormat('es-HN', { style: 'currency', currency: 'HNL' }).format(amount)
const shortDate = (value: string) => new Intl.DateTimeFormat('es-HN', { dateStyle: 'medium' }).format(new Date(value))

function providerName(id: number) {
  return providers.value.find((provider) => provider.id === id)?.nombre ?? `Proveedor #${id}`
}

function addLine(product: Product) {
  if (quantity.value <= 0) return
  const existing = lines.value.find((line) => line.product.id === product.id)
  if (existing) existing.quantity += quantity.value
  else lines.value.push({ product, quantity: quantity.value })
  productId.value = null
  quantity.value = 1
  error.value = ''
}

function changeQuantity(line: PurchaseLine, amount: number) {
  const next = line.quantity + amount
  if (next <= 0) lines.value = lines.value.filter((item) => item !== line)
  else line.quantity = next
}

async function load() {
  loading.value = true
  error.value = ''
  const responses = await Promise.allSettled([
    purchaseService.list({ pagina: 1, cantidad: 50 }),
    providerService.list(),
    cashRegisterService.getOpen(),
  ])
  if (responses[0].status === 'fulfilled') result.value = responses[0].value
  else error.value = responses[0].reason instanceof Error ? responses[0].reason.message : 'No se pudieron cargar las compras.'
  if (responses[1].status === 'fulfilled') providers.value = responses[1].value
  register.value = responses[2].status === 'fulfilled' ? responses[2].value : null
  loading.value = false
}

async function createPurchase() {
  if (!auth.user?.id || !providerId.value || !register.value || !lines.value.length) return
  saving.value = true
  error.value = ''
  try {
    await purchaseService.create({
      usuarioId: auth.user.id,
      cajaId: register.value.id,
      proveedorId: providerId.value,
      detalles: lines.value.map((line) => ({ productoId: line.product.id, cantidad: line.quantity })),
    })
    notice.value = 'Compra registrada correctamente.'
    lines.value = []
    providerId.value = null
    await load()
  } catch (cause) {
    error.value = cause instanceof Error ? cause.message : 'No se pudo registrar la compra.'
  } finally { saving.value = false }
}

onMounted(load)
</script>

<template>
  <section class="fade-up purchases-page">
    <div class="page-heading">
      <div>
        <p class="eyebrow">Abastecimiento</p>
        <h1>Compras</h1>
        <p>Recibe mercancía y mantén tu inventario al día.</p>
      </div>
      <span class="status-pill" :class="{ closed: !register }">
        {{ register ? `Caja #${register.id} abierta` : 'Caja cerrada' }}
      </span>
    </div>
    <div v-if="error" class="alert-box" style="margin-bottom:12px">{{ error }}</div>
    <div v-if="notice" class="register-notice">{{ notice }}</div>
    <div class="purchase-layout">
      <section class="panel">
        <div class="panel-header">
          <div>
            <h2>Nueva compra</h2>
            <p>Selecciona proveedor y agrega los productos recibidos.</p>
          </div>
          <span class="register-history-mark"><ClipboardList :size="17" /></span>
        </div>
        <div class="purchase-form">
          <label class="form-field">
            Proveedor
            <select v-model.number="providerId" class="field" required>
              <option :value="null" disabled>Selecciona un proveedor</option>
              <option v-for="provider in providers" :key="provider.id" :value="provider.id">{{ provider.nombre }}</option>
            </select>
          </label>

          <div class="purchase-add-row">
            <ProductSearch
              v-model="productId"
              :placeholder="'Buscar producto por nombre o código...'"
              :show-stock="true"
              :show-price="'compra'"
              :page-size="15"
              @select="addLine"
              style="flex: 1;"
            />
            <label class="form-field purchase-quantity">
              Cantidad
              <input v-model.number="quantity" class="field" type="number" min="0.01" step="any" />
            </label>
          </div>

          <div v-if="lines.length" class="purchase-lines">
            <div class="purchase-lines-head">
              <span>Producto</span>
              <span>Cantidad</span>
              <span>Costo unitario</span>
              <span>Importe</span>
              <span></span>
            </div>
            <div v-for="line in lines" :key="line.product.id" class="purchase-line">
              <div class="purchase-line-product">
                <span class="purchase-product-icon"><Package :size="16" /></span>
                <span>
                  <strong>{{ line.product.nombre }}</strong>
                  <small>{{ line.product.codigo }}</small>
                </span>
              </div>
              <div class="purchase-line-quantity">
                <button aria-label="Reducir cantidad" @click="changeQuantity(line,-1)"><Minus :size="12" /></button>
                <span>{{ line.quantity }}</span>
                <button aria-label="Aumentar cantidad" @click="changeQuantity(line,1)"><Plus :size="12" /></button>
              </div>
              <span>{{ currency(line.product.precioCompra) }}</span>
              <strong>{{ currency(line.product.precioCompra * line.quantity) }}</strong>
              <button class="table-action delete" :aria-label="`Quitar ${line.product.nombre}`" @click="changeQuantity(line,-line.quantity)">
                <Trash2 :size="14" />
              </button>
            </div>
          </div>
          <div v-else class="purchase-lines-empty">
            <Package :size="21" />
            <strong>Agrega productos a la compra</strong>
            <span>Los artículos recibidos aparecerán aquí.</span>
          </div>
          <div class="purchase-form-footer">
            <div class="purchase-estimate">
              <span>Total estimado</span>
              <strong>{{ currency(total) }}</strong>
              <small>Incluye impuesto estimado del 15%; el API calcula el total final.</small>
            </div>
            <button class="btn" :disabled="saving || !register || !providerId || !lines.length" @click="createPurchase">
              {{ saving ? 'Guardando…' : 'Registrar compra' }}
              <ArrowRight :size="16" />
            </button>
          </div>
          <div v-if="!register" class="purchase-cash-hint">
            Abre una caja antes de registrar compras.
            <RouterLink to="/cash-register">Ir a Caja →</RouterLink>
          </div>
        </div>
      </section>
      <aside class="panel purchase-summary-panel">
        <div class="panel-header">
          <div>
            <h2>Resumen</h2>
            <p>Actividad de abastecimiento</p>
          </div>
          <span class="register-history-mark"><Package :size="17" /></span>
        </div>
        <div class="purchase-summary-body">
          <strong>{{ result?.total ?? 0 }}</strong>
          <span>compras registradas</span>
          <div>
            <span>Proveedor seleccionado</span>
            <b>{{ providers.find((provider) => provider.id === providerId)?.nombre ?? '—' }}</b>
          </div>
          <div>
            <span>Líneas en esta compra</span>
            <b>{{ lines.length }}</b>
          </div>
        </div>
      </aside>
    </div>
    <section class="panel purchase-history">
      <div class="panel-header">
        <div>
          <h2>Historial de compras</h2>
          <p>{{ result?.total ?? 0 }} registros</p>
        </div>
        <button class="btn btn-secondary" :disabled="loading" @click="load">
          <RefreshCw :size="15" /> Actualizar
        </button>
      </div>
      <div v-if="loading" class="loading-state">Cargando compras…</div>
      <div v-else class="table-wrap">
        <table class="data-table">
          <thead>
            <tr>
              <th>Compra</th>
              <th>Fecha</th>
              <th>Proveedor</th>
              <th>Caja</th>
              <th>Estado</th>
              <th>Total</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="purchase in result?.items ?? []" :key="purchase.id">
              <td class="row-primary">#{{ purchase.id }}</td>
              <td>{{ shortDate(purchase.fecha) }}</td>
              <td>{{ providerName(purchase.proveedorId) }}</td>
              <td>#{{ purchase.cajaId }}</td>
              <td><span class="status-pill">{{ purchase.estado === 1 ? 'Completada' : 'Registrada' }}</span></td>
              <td class="row-primary">{{ currency(purchase.total) }}</td>
            </tr>
          </tbody>
        </table>
        <div v-if="!result?.items.length" class="empty-state">
          <ClipboardList :size="24" />
          <strong>Aún no hay compras</strong>
          <span>Las compras confirmadas se mostrarán aquí.</span>
        </div>
      </div>
    </section>
  </section>
</template>