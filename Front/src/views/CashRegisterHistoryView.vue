<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { Eye, FileText, Package, RefreshCw, Wallet, ArrowRight, ArrowLeft } from '@lucide/vue'
import { cashRegisterService, saleService, purchaseService, productService } from '@/Service'
import { useAuthStore } from '@/Store'
import type { CashRegister } from '@/Type/CashRegister'
import type { Sale } from '@/Type/Sale'
import type { Purchase } from '@/Type/Purchase'
import type { SaleDetail } from '@/Type/Sale'
import type { PurchaseDetail } from '@/Type/Purchase'
import type { Product } from '@/Type/Product'

const auth = useAuthStore()
const registers = ref<CashRegister[]>([])
const loading = ref(false)
const error = ref('')
const notice = ref('')

// Selected register details
const selectedRegister = ref<CashRegister | null>(null)
const registerSales = ref<Sale[]>([])
const registerPurchases = ref<Purchase[]>([])
const detailsLoading = ref(false)
const detailsError = ref('')

// Selected sale/purchase details
const selectedSale = ref<Sale | null>(null)
const saleDetails = ref<SaleDetail[]>([])
const selectedPurchase = ref<Purchase | null>(null)
const purchaseDetails = ref<PurchaseDetail[]>([])
const productNames = ref<Record<number, string>>({})

const currency = (amount: number) => new Intl.NumberFormat('es-HN', { style: 'currency', currency: 'HNL' }).format(amount)
const dateTime = (value: string | null) => value ? new Intl.DateTimeFormat('es-HN', { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(value)) : '—'
const shortDate = (value: string) => new Intl.DateTimeFormat('es-HN', { dateStyle: 'medium' }).format(new Date(value))

function paymentMethod(method: number | null) {
  if (method === 1) return 'Efectivo'
  if (method === 2) return 'Tarjeta'
  if (method === 3) return 'Transferencia'
  return 'No especificado'
}

function paymentMethodIcon(method: number | null) {
  if (method === 1) return '💵'
  if (method === 2) return '💳'
  if (method === 3) return '🏦'
  return '❓'
}

async function load() {
  loading.value = true
  error.value = ''
  try {
    const result = await cashRegisterService.list({ pagina: 1, cantidad: 100 })
    registers.value = result.items
  } catch (cause) {
    error.value = cause instanceof Error ? cause.message : 'No se pudo cargar el historial de cajas.'
  } finally {
    loading.value = false
  }
}

async function showRegisterDetails(register: CashRegister) {
  selectedRegister.value = register
  registerSales.value = []
  registerPurchases.value = []
  detailsError.value = ''
  detailsLoading.value = true
  
  try {
    const [sales, purchases] = await Promise.allSettled([
      saleService.getByCashRegister(register.id),
      purchaseService.getByCashRegister(register.id)
    ])
    
    if (sales.status === 'fulfilled') registerSales.value = sales.value
    if (purchases.status === 'fulfilled') registerPurchases.value = purchases.value
    
    if (sales.status === 'rejected' || purchases.status === 'rejected') {
      detailsError.value = 'No se pudieron cargar todas las transacciones.'
    }
  } catch (cause) {
    detailsError.value = cause instanceof Error ? cause.message : 'Error al cargar detalles.'
  } finally {
    detailsLoading.value = false
  }
}

function closeRegisterDetails() {
  selectedRegister.value = null
  registerSales.value = []
  registerPurchases.value = []
  detailsError.value = ''
  selectedSale.value = null
  saleDetails.value = []
  selectedPurchase.value = null
  purchaseDetails.value = []
}

async function showSaleDetails(sale: Sale) {
  selectedSale.value = sale
  saleDetails.value = []
  detailsError.value = ''
  detailsLoading.value = true
  
  try {
    const details = await saleService.getDetails(sale.id)
    saleDetails.value = details
    
    // Fetch product names for details
    const productIds = [...new Set(details.map(d => d.productoId))]
    for (const pid of productIds) {
      try {
        const product = await productService.getById(pid)
        productNames.value[pid] = product.nombre
      } catch {
        productNames.value[pid] = `Producto #${pid}`
      }
    }
  } catch (cause) {
    detailsError.value = cause instanceof Error ? cause.message : 'No se pudieron cargar los detalles de la venta.'
  } finally {
    detailsLoading.value = false
  }
}

async function showPurchaseDetails(purchase: Purchase) {
  selectedPurchase.value = purchase
  purchaseDetails.value = []
  detailsError.value = ''
  detailsLoading.value = true
  
  try {
    const details = await purchaseService.getDetails(purchase.id)
    purchaseDetails.value = details
    
    // Fetch product names for details
    const productIds = [...new Set(details.map(d => d.productoId))]
    for (const pid of productIds) {
      try {
        const product = await productService.getById(pid)
        productNames.value[pid] = product.nombre
      } catch {
        productNames.value[pid] = `Producto #${pid}`
      }
    }
  } catch (cause) {
    detailsError.value = cause instanceof Error ? cause.message : 'No se pudieron cargar los detalles de la compra.'
  } finally {
    detailsLoading.value = false
  }
}

function closeTransactionDetails() {
  selectedSale.value = null
  saleDetails.value = []
  selectedPurchase.value = null
  purchaseDetails.value = []
  detailsError.value = ''
}

function purchaseStatus(status: number) {
  return status === 1 ? 'Completada' : 'Registrada'
}

function getProductName(productId: number) {
  return productNames.value[productId] || `Producto #${productId}`
}

const salesByMethod = computed(() => {
  const grouped: Record<number, Sale[]> = {}
  for (const sale of registerSales.value) {
    const method = sale.metodoPago ?? 0
    if (!grouped[method]) grouped[method] = []
    grouped[method].push(sale)
  }
  return grouped
})

const salesTotal = computed(() => 
  registerSales.value.reduce((sum, s) => sum + s.total, 0)
)

const purchasesTotal = computed(() => 
  registerPurchases.value.reduce((sum, p) => sum + p.total, 0)
)

onMounted(load)
</script>

<template>
  <section class="fade-up register-history-page">
    <div class="page-heading">
      <div>
        <p class="eyebrow">Historial por caja</p>
        <h1>Cajas con Ventas y Compras</h1>
        <p>Visualiza todas las transacciones agrupadas por turno de caja con sus detalles.</p>
      </div>
      <button class="btn btn-secondary" :disabled="loading" @click="load">
        <RefreshCw :size="15"/> Actualizar
      </button>
    </div>
    
    <div v-if="error" class="alert-box register-alert">{{ error }}</div>
    <div v-if="notice" class="register-notice">{{ notice }}</div>

    <!-- Main Registers List -->
    <section v-if="!selectedRegister" class="panel">
      <div class="panel-header">
        <div>
          <h2>Historial de Cajas</h2>
          <p>{{ registers.length }} turnos encontrados</p>
        </div>
        <span class="register-history-mark"><Wallet :size="17"/></span>
      </div>
      
      <div v-if="loading" class="loading-state">Cargando historial…</div>
      <div v-else class="table-wrap">
        <table class="data-table">
          <thead>
            <tr>
              <th>Apertura</th>
              <th>Cierre</th>
              <th>Usuario</th>
              <th>Fondo Inicial</th>
              <th>Fondo Final</th>
              <th>Estado</th>
              <th>Ventas</th>
              <th>Compras</th>
              <th>Total Ventas</th>
              <th>Total Compras</th>
              <th>Acción</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="register in registers" :key="register.id">
              <td>
                <strong class="table-main-text">{{ dateTime(register.fechaApertura) }}</strong>
                <small class="table-subtext">Caja #{{ register.id }}</small>
              </td>
              <td>{{ dateTime(register.fechaCierre) }}</td>
              <td>Usuario #{{ register.usuarioId }}</td>
              <td class="register-amount">{{ currency(register.montoInicial) }}</td>
              <td>{{ register.montoFinal === null ? '—' : currency(register.montoFinal) }}</td>
              <td>
                <span class="status-pill" :class="{closed: !register.estaAbierta}">
                  {{ register.estaAbierta ? 'Abierta' : 'Cerrada' }}
                </span>
              </td>
              <td>{{ 0 }}</td>
              <td>{{ 0 }}</td>
              <td class="register-amount">{{ currency(0) }}</td>
              <td class="register-amount">{{ currency(0) }}</td>
              <td>
                <button 
                  class="table-action register-detail-button" 
                  :aria-label="`Ver detalles de caja ${register.id}`" 
                  @click="showRegisterDetails(register)"
                  :disabled="detailsLoading"
                >
                  <Eye :size="15"/><span>Ver</span>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
        <div v-if="!registers.length" class="empty-state">
          <Wallet :size="25"/>
          <strong>Aún no hay cajas</strong>
          <span>Al abrir un turno, aparecerá en este historial.</span>
        </div>
      </div>
    </section>

    <!-- Register Details with Sales and Purchases -->
    <div v-if="selectedRegister" class="dialog-backdrop" @click.self="closeRegisterDetails">
      <section class="checkout-dialog register-details-dialog" role="dialog" aria-modal="true" style="max-width: 1200px; width: 95vw;">
        <header class="checkout-dialog-header">
          <div>
            <span class="dialog-step-label">HISTORIAL DE CAJA #{{ selectedRegister.id }}</span>
            <h2>Detalle de Caja</h2>
            <p>Apertura {{ dateTime(selectedRegister.fechaApertura) }} · {{ selectedRegister.estaAbierta ? 'Abierta' : 'Cerrada' }}</p>
          </div>
          <button class="dialog-close" aria-label="Cerrar detalle" @click="closeRegisterDetails"><X :size="18"/></button>
        </header>

        <div class="register-details-summary" style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 16px; padding: 16px; background: var(--color-surface); border-radius: 8px; margin: 16px;">
          <div><span>Estado</span><strong><span class="status-pill" :class="{closed: !selectedRegister.estaAbierta}">{{ selectedRegister.estaAbierta ? 'Abierta' : 'Cerrada' }}</span></strong></div>
          <div><span>Usuario</span><strong>Usuario #{{ selectedRegister.usuarioId }}</strong></div>
          <div><span>Fondo inicial</span><strong>{{ currency(selectedRegister.montoInicial) }}</strong></div>
          <div><span>Fondo final</span><strong>{{ selectedRegister.montoFinal === null ? '—' : currency(selectedRegister.montoFinal) }}</strong></div>
          <div><span>Ventas totales</span><strong>{{ currency(salesTotal) }}</strong></div>
          <div><span>Compras totales</span><strong>{{ currency(purchasesTotal) }}</strong></div>
        </div>

        <div v-if="detailsLoading" class="loading-state" style="padding: 32px;">Cargando transacciones…</div>
        <div v-else-if="detailsError" class="alert-box" style="margin: 16px;">{{ detailsError }}</div>

        <template v-else>
          <!-- SALES SECTION -->
          <div class="panel" style="margin: 16px;">
            <div class="panel-header">
              <div>
                <h2>
                  <FileText :size="20" style="vertical-align: middle; margin-right: 8px;"/>
                  Ventas ({{ registerSales.length }})
                </h2>
                <p>Total: {{ currency(salesTotal) }}</p>
              </div>
            </div>
            
            <div v-if="registerSales.length === 0" class="empty-state" style="padding: 32px;">
              <FileText :size="24"/>
              <strong>Sin ventas</strong>
              <span>Esta caja no tiene ventas registradas.</span>
            </div>
            
            <div v-else class="table-wrap">
              <table class="data-table">
                <thead>
                  <tr>
                    <th>N°</th>
                    <th>Fecha</th>
                    <th>Cliente</th>
                    <th>Pago</th>
                    <th>Estado</th>
                    <th>Total</th>
                    <th>Acción</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="sale in registerSales" :key="sale.id">
                    <td class="row-primary">#{{ sale.id }}</td>
                    <td>{{ shortDate(sale.fecha) }}</td>
                    <td>{{ sale.clienteId ? `Cliente #${sale.clienteId}` : 'Cliente ocasional' }}</td>
                    <td>
                      <span style="display: inline-flex; align-items: center; gap: 4px;">
                        {{ paymentMethodIcon(sale.metodoPago) }} {{ paymentMethod(sale.metodoPago) }}
                      </span>
                    </td>
                    <td><span class="status-pill" :class="{ alert: sale.estado === 2 }">{{ sale.estado === 2 ? 'Cancelada' : 'Completada' }}</span></td>
                    <td class="row-primary">{{ currency(sale.total) }}</td>
                    <td>
                      <button class="table-action" @click="showSaleDetails(sale)" :disabled="detailsLoading">
                        <Eye :size="15"/><span>Detalle</span>
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- PURCHASES SECTION -->
          <div class="panel" style="margin: 16px;">
            <div class="panel-header">
              <div>
                <h2>
                  <Package :size="20" style="vertical-align: middle; margin-right: 8px;"/>
                  Compras ({{ registerPurchases.length }})
                </h2>
                <p>Total: {{ currency(purchasesTotal) }}</p>
              </div>
            </div>
            
            <div v-if="registerPurchases.length === 0" class="empty-state" style="padding: 32px;">
              <Package :size="24"/>
              <strong>Sin compras</strong>
              <span>Esta caja no tiene compras registradas.</span>
            </div>
            
            <div v-else class="table-wrap">
              <table class="data-table">
                <thead>
                  <tr>
                    <th>N°</th>
                    <th>Fecha</th>
                    <th>Proveedor</th>
                    <th>Estado</th>
                    <th>Total</th>
                    <th>Acción</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="purchase in registerPurchases" :key="purchase.id">
                    <td class="row-primary">#{{ purchase.id }}</td>
                    <td>{{ shortDate(purchase.fecha) }}</td>
                    <td>Proveedor #{{ purchase.proveedorId }}</td>
                    <td><span class="status-pill">{{ purchaseStatus(purchase.estado) }}</span></td>
                    <td class="row-primary">{{ currency(purchase.total) }}</td>
                    <td>
                      <button class="table-action" @click="showPurchaseDetails(purchase)" :disabled="detailsLoading">
                        <Eye :size="15"/><span>Detalle</span>
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </template>

        <footer class="checkout-dialog-actions">
          <button class="btn btn-secondary" @click="closeRegisterDetails">Cerrar</button>
        </footer>
      </section>
    </div>

    <!-- Sale Details Modal -->
    <div v-if="selectedSale" class="dialog-backdrop" @click.self="closeTransactionDetails">
      <section class="checkout-dialog sale-details-dialog" role="dialog" aria-modal="true" style="max-width: 800px;">
        <header class="checkout-dialog-header">
          <div>
            <span class="dialog-step-label">DETALLE DE VENTA</span>
            <h2>Venta #{{ selectedSale.id }}</h2>
            <p>{{ shortDate(selectedSale.fecha) }} · {{ paymentMethod(selectedSale.metodoPago) }}</p>
          </div>
          <button class="dialog-close" aria-label="Cerrar detalle" @click="closeTransactionDetails"><X :size="18"/></button>
        </header>
        
        <div class="sale-details-meta" style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 12px; padding: 16px; background: var(--color-surface); border-radius: 8px; margin: 16px;">
          <div><span>Cliente</span><strong>{{ selectedSale.clienteId ? `Cliente #${selectedSale.clienteId}` : 'Cliente ocasional' }}</strong></div>
          <div><span>Caja</span><strong>#{{ selectedSale.cajaId }}</strong></div>
          <div><span>Usuario</span><strong>#{{ selectedSale.usuarioId }}</strong></div>
          <div><span>Método pago</span><strong>{{ paymentMethod(selectedSale.metodoPago) }}</strong></div>
          <div><span>Estado</span><strong>{{ selectedSale.estado === 2 ? 'Cancelada' : 'Completada' }}</strong></div>
        </div>
        
        <div class="sale-details-content">
          <div class="sale-details-section-title">
            <div><h3>Productos</h3><p>{{ saleDetails.length }} líneas en esta venta</p></div>
          </div>
          <div v-if="detailsLoading" class="loading-state">Cargando productos…</div>
          <div v-else-if="detailsError" class="alert-box">{{ detailsError }}</div>
          <div v-else class="table-wrap">
            <table class="data-table">
              <thead>
                <tr><th>Producto</th><th>Cantidad</th><th>Precio unitario</th><th>Importe</th></tr>
              </thead>
              <tbody>
                <tr v-for="line in saleDetails" :key="line.id">
                  <td><strong class="table-main-text">{{ getProductName(line.productoId) }}</strong></td>
                  <td>{{ line.cantidad }}</td>
                  <td>{{ currency(line.precioUnitario) }}</td>
                  <td class="register-amount">{{ currency(line.subtotal) }}</td>
                </tr>
              </tbody>
            </table>
            <div v-if="!saleDetails.length" class="empty-state">Esta venta no tiene líneas de detalle.</div>
          </div>
        </div>
        
        <div class="sale-details-totals" style="display: flex; justify-content: flex-end; gap: 24px; padding: 16px; border-top: 1px solid var(--color-border); margin-top: 16px;">
          <div><span>Subtotal</span><strong>{{ currency(selectedSale.subtotal) }}</strong></div>
          <div><span>Impuesto</span><strong>{{ currency(selectedSale.impuesto) }}</strong></div>
          <div class="sale-details-total"><span>Total</span><strong>{{ currency(selectedSale.total) }}</strong></div>
        </div>
        
        <footer class="checkout-dialog-actions">
          <button class="btn btn-secondary" @click="closeTransactionDetails">Cerrar</button>
        </footer>
      </section>
    </div>

    <!-- Purchase Details Modal -->
    <div v-if="selectedPurchase" class="dialog-backdrop" @click.self="closeTransactionDetails">
      <section class="checkout-dialog sale-details-dialog" role="dialog" aria-modal="true" style="max-width: 800px;">
        <header class="checkout-dialog-header">
          <div>
            <span class="dialog-step-label">DETALLE DE COMPRA</span>
            <h2>Compra #{{ selectedPurchase.id }}</h2>
            <p>{{ shortDate(selectedPurchase.fecha) }}</p>
          </div>
          <button class="dialog-close" aria-label="Cerrar detalle" @click="closeTransactionDetails"><X :size="18"/></button>
        </header>
        
        <div class="sale-details-meta" style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 12px; padding: 16px; background: var(--color-surface); border-radius: 8px; margin: 16px;">
          <div><span>Proveedor</span><strong>Proveedor #{{ selectedPurchase.proveedorId }}</strong></div>
          <div><span>Caja</span><strong>#{{ selectedPurchase.cajaId }}</strong></div>
          <div><span>Usuario</span><strong>#{{ selectedPurchase.usuarioId }}</strong></div>
          <div><span>Estado</span><strong>{{ purchaseStatus(selectedPurchase.estado) }}</strong></div>
        </div>
        
        <div class="sale-details-content">
          <div class="sale-details-section-title">
            <div><h3>Productos</h3><p>{{ purchaseDetails.length }} líneas en esta compra</p></div>
          </div>
          <div v-if="detailsLoading" class="loading-state">Cargando productos…</div>
          <div v-else-if="detailsError" class="alert-box">{{ detailsError }}</div>
          <div v-else class="table-wrap">
            <table class="data-table">
              <thead>
                <tr><th>Producto</th><th>Cantidad</th><th>Costo unitario</th><th>Importe</th></tr>
              </thead>
              <tbody>
                <tr v-for="line in purchaseDetails" :key="line.id">
                  <td><strong class="table-main-text">{{ getProductName(line.productoId) }}</strong></td>
                  <td>{{ line.cantidad }}</td>
                  <td>{{ currency(line.precioUnitario) }}</td>
                  <td class="register-amount">{{ currency(line.subtotal) }}</td>
                </tr>
              </tbody>
            </table>
            <div v-if="!purchaseDetails.length" class="empty-state">Esta compra no tiene líneas de detalle.</div>
          </div>
        </div>
        
        <div class="sale-details-totals" style="display: flex; justify-content: flex-end; gap: 24px; padding: 16px; border-top: 1px solid var(--color-border); margin-top: 16px;">
          <div><span>Subtotal</span><strong>{{ currency(selectedPurchase.subtotal) }}</strong></div>
          <div><span>Impuesto</span><strong>{{ currency(selectedPurchase.impuesto) }}</strong></div>
          <div class="sale-details-total"><span>Total</span><strong>{{ currency(selectedPurchase.total) }}</strong></div>
        </div>
        
        <footer class="checkout-dialog-actions">
          <button class="btn btn-secondary" @click="closeTransactionDetails">Cerrar</button>
        </footer>
      </section>
    </div>
  </section>
</template>