<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { CircleAlert, Eye, FileText, Package, RefreshCw, Trash2, X, Calendar, X as XIcon, Download } from '@lucide/vue'
import { productService, purchaseService } from '@/Service'
import { useAuthStore } from '@/Store'
import { usePdfExport, formatCurrencyPdf, formatDateTimePdf } from '@/composables/usePdfExport'
import type { PagedResult } from '@/Type/Common'
import type { Purchase, PurchaseDetail } from '@/Type/Purchase'

interface PurchaseDetailRow extends PurchaseDetail {
  productName: string
  productCode?: string
}

const auth = useAuthStore()
const result = ref<PagedResult<Purchase> | null>(null)
const selectedPurchase = ref<Purchase | null>(null)
const purchaseDetails = ref<PurchaseDetailRow[]>([])
const purchasePendingDelete = ref<Purchase | null>(null)
const loading = ref(false)
const detailsLoading = ref(false)
const deletingId = ref<number | null>(null)
const error = ref('')
const notice = ref('')
const detailsError = ref('')
const deleteError = ref('')
const currency = (amount: number) => new Intl.NumberFormat('es-HN', { style: 'currency', currency: 'HNL' }).format(amount)
const shortDate = (value: string) => new Intl.DateTimeFormat('es-HN', { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(value))

// Date filter state
const fechaDesde = ref('')
const fechaHasta = ref('')

function purchaseStatus(status: number) {
  return status === 1 ? 'Completada' : 'Registrada'
}

async function load() {
  loading.value = true
  error.value = ''
  try {
    result.value = await purchaseService.list({ 
      pagina: 1, 
      cantidad: 50,
      fechaDesde: fechaDesde.value || undefined,
      fechaHasta: fechaHasta.value || undefined
    })
  } catch (cause) {
    error.value = cause instanceof Error ? cause.message : 'No se pudieron cargar las compras.'
  } finally {
    loading.value = false
  }
}

const { exportToPdf } = usePdfExport()

function exportPurchasesToPdf() {
  if (!result.value?.items.length) return
  const dateRange = fechaDesde.value || fechaHasta.value
    ? `${fechaDesde.value ? formatDateTimePdf(fechaDesde.value) : 'Inicio'} - ${fechaHasta.value ? formatDateTimePdf(fechaHasta.value) : 'Hoy'}`
    : 'Todas las fechas'

  exportToPdf({
    title: 'Historial de Compras',
    subtitle: `Filtro: ${dateRange} · Total: ${result.value.items.length} compras`,
    columns: [
      { header: 'N°', dataKey: 'id', width: 15 },
      { header: 'Fecha y Hora', dataKey: 'fecha', width: 35 },
      { header: 'Proveedor', dataKey: 'proveedor', width: 35 },
      { header: 'Caja', dataKey: 'caja', width: 15 },
      { header: 'Estado', dataKey: 'estado', width: 25 },
      { header: 'Total', dataKey: 'total', width: 30 },
    ],
    data: result.value.items.map(p => ({
      id: `#${p.id}`,
      fecha: formatDateTimePdf(p.fecha),
      proveedor: `Proveedor #${p.proveedorId}`,
      caja: `#${p.cajaId}`,
      estado: p.estado === 1 ? 'Completada' : 'Registrada',
      total: formatCurrencyPdf(p.total),
    })),
    fileName: `compras-${fechaDesde.value || 'todas'}-${fechaHasta.value || 'hoy'}.pdf`,
  })
}

function clearDateFilter() {
  fechaDesde.value = ''
  fechaHasta.value = ''
  load()
}

function hasDateFilter() {
  return !!fechaDesde.value || !!fechaHasta.value
}

async function showDetails(purchase: Purchase) {
  selectedPurchase.value = purchase
  purchaseDetails.value = []
  detailsError.value = ''
  detailsLoading.value = true

  try {
    const details = await purchaseService.getDetails(purchase.id)
    purchaseDetails.value = await Promise.all(details.map(async (detail) => {
      try {
        const product = await productService.getById(detail.productoId)
        return { ...detail, productName: product.nombre, productCode: product.codigo }
      } catch {
        return { ...detail, productName: 'Producto #' + detail.productoId }
      }
    }))
  } catch (cause) {
    detailsError.value = cause instanceof Error ? cause.message : 'No se pudieron cargar los detalles.'
  } finally {
    detailsLoading.value = false
  }
}

function closeDetails() {
  selectedPurchase.value = null
  purchaseDetails.value = []
  detailsError.value = ''
}

function requestDelete(purchase: Purchase) {
  if (!auth.isAdmin) return
  deleteError.value = ''
  purchasePendingDelete.value = purchase
}

function cancelDelete() {
  if (deletingId.value !== null) return
  purchasePendingDelete.value = null
  deleteError.value = ''
}

async function deletePurchase() {
  const purchase = purchasePendingDelete.value
  if (!auth.isAdmin || !purchase) return
  deletingId.value = purchase.id
  deleteError.value = ''
  error.value = ''
  try {
    await purchaseService.remove(purchase.id)
    notice.value = 'La compra #' + purchase.id + ' fue eliminada.'
    purchasePendingDelete.value = null
    if (selectedPurchase.value?.id === purchase.id) closeDetails()
    await load()
  } catch (cause) {
    deleteError.value = cause instanceof Error ? cause.message : 'No se pudo eliminar la compra.'
  } finally {
    deletingId.value = null
  }
}

onMounted(load)
</script>

<template>
  <section class="fade-up purchases-history-page">
    <div class="page-heading">
      <div><p class="eyebrow">Historial</p><h1>Historial de Compras</h1><p>Consulta operaciones de abastecimiento y revisa sus productos.</p></div>
      <RouterLink class="btn" to="/purchases">Nueva compra <span aria-hidden="true">→</span></RouterLink>
    </div>
    <div v-if="error" class="alert-box" style="margin-bottom: 16px">{{ error }}</div>
    <div v-if="notice" class="register-notice">{{ notice }}</div>

    <!-- Date Filter -->
    <div class="filter-bar">
      <div class="filter-group">
        <label for="fechaDesde">Desde</label>
        <div class="date-input-wrapper">
          <Calendar :size="16" class="date-input-icon" />
          <input 
            id="fechaDesde" 
            type="date" 
            v-model="fechaDesde" 
            class="date-input"
            @change="load"
          />
        </div>
      </div>
      <div class="filter-group">
        <label for="fechaHasta">Hasta</label>
        <div class="date-input-wrapper">
          <Calendar :size="16" class="date-input-icon" />
          <input 
            id="fechaHasta" 
            type="date" 
            v-model="fechaHasta" 
            class="date-input"
            @change="load"
          />
        </div>
      </div>
      <button 
        v-if="hasDateFilter()" 
        class="btn btn-secondary btn-sm clear-filter" 
        @click="clearDateFilter"
        title="Limpiar filtros"
      >
        <XIcon :size="14" /> Limpiar
      </button>
    </div>

    <section class="panel">
      <div class="panel-header">
        <div><h2>Transacciones</h2><p>{{ result?.total ?? 0 }} compras registradas</p></div>
        <div class="panel-actions">
          <button class="btn btn-secondary" :disabled="loading" @click="load"><RefreshCw :size="15"/> Actualizar</button>
          <button v-if="result?.items?.length" class="btn btn-primary" @click="exportPurchasesToPdf"><Download :size="15"/> Exportar PDF</button>
        </div>
      </div>
      <div v-if="loading" class="loading-state">Cargando compras...</div>
      <div v-else class="table-wrap"><table class="data-table"><thead><tr><th>Numero</th><th>Fecha</th><th>Proveedor</th><th>Caja</th><th>Estado</th><th>Total</th><th>Acciones</th></tr></thead><tbody>
        <tr v-for="purchase in result?.items ?? []" :key="purchase.id"><td class="row-primary">#{{ purchase.id }}</td><td>{{ shortDate(purchase.fecha) }}</td><td>Proveedor #{{ purchase.proveedorId }}</td><td>#{{ purchase.cajaId }}</td><td><span class="status-pill">{{ purchaseStatus(purchase.estado) }}</span></td><td class="row-primary">{{ currency(purchase.total) }}</td><td><div class="table-actions"><button class="table-action" :aria-label="'Ver compra ' + purchase.id" @click="showDetails(purchase)"><Eye :size="15"/><span>Detalle</span></button><button v-if="auth.isAdmin" class="table-action delete" :aria-label="'Eliminar compra ' + purchase.id" :disabled="deletingId === purchase.id" @click="requestDelete(purchase)"><Trash2 :size="14"/><span>Eliminar</span></button></div></td></tr>
      </tbody></table><div v-if="!result?.items.length" class="empty-state"><FileText :size="24"/><strong>Aun no hay compras</strong><span>Las compras realizadas apareceran en este historial.</span></div></div>
    </section>

    <div v-if="selectedPurchase" class="dialog-backdrop" @click.self="closeDetails"><section class="checkout-dialog purchase-details-dialog" role="dialog" aria-modal="true" aria-labelledby="purchase-details-title">
      <header class="checkout-dialog-header"><div><span class="dialog-step-label">DETALLE DE OPERACION</span><h2 id="purchase-details-title">Compra #{{ selectedPurchase.id }}</h2><p>{{ shortDate(selectedPurchase.fecha) }}</p></div><button class="dialog-close" aria-label="Cerrar detalle" @click="closeDetails"><X :size="18"/></button></header>
      <div class="sale-details-meta"><div><span>Proveedor</span><strong>Proveedor #{{ selectedPurchase.proveedorId }}</strong></div><div><span>Caja</span><strong>#{{ selectedPurchase.cajaId }}</strong></div><div><span>Usuario</span><strong>#{{ selectedPurchase.usuarioId }}</strong></div><div><span>Estado</span><strong>{{ purchaseStatus(selectedPurchase.estado) }}</strong></div></div>
      <div class="sale-details-content"><div class="sale-details-section-title"><div><h3>Productos</h3><p>{{ purchaseDetails.length }} lineas en esta compra</p></div></div>
        <div v-if="detailsLoading" class="loading-state">Cargando productos...</div><div v-else-if="detailsError" class="alert-box">{{ detailsError }}</div>
        <div v-else class="table-wrap"><table class="data-table"><thead><tr><th>Producto</th><th>Cantidad</th><th>Costo unitario</th><th>Importe</th></tr></thead><tbody><tr v-for="line in purchaseDetails" :key="line.id"><td><strong class="table-main-text">{{ line.productName }}</strong><small v-if="line.productCode" class="table-subtext">{{ line.productCode }}</small></td><td>{{ line.cantidad }}</td><td>{{ currency(line.precioUnitario) }}</td><td class="register-amount">{{ currency(line.subtotal) }}</td></tr></tbody></table><div v-if="!purchaseDetails.length" class="empty-state">Esta compra no tiene lineas de detalle.</div></div>
      </div>
      <div class="sale-details-totals"><div><span>Subtotal</span><strong>{{ currency(selectedPurchase.subtotal) }}</strong></div><div><span>Impuesto</span><strong>{{ currency(selectedPurchase.impuesto) }}</strong></div><div class="sale-details-total"><span>Total</span><strong>{{ currency(selectedPurchase.total) }}</strong></div></div>
      <footer class="checkout-dialog-actions"><button v-if="auth.isAdmin" class="btn btn-danger" :disabled="deletingId === selectedPurchase.id" @click="requestDelete(selectedPurchase)"><Trash2 :size="14"/> Eliminar compra</button><button class="btn btn-secondary" @click="closeDetails">Cerrar</button></footer>
    </section></div>

    <div v-if="purchasePendingDelete" class="dialog-backdrop confirm-delete-backdrop" @click.self="cancelDelete"><section class="checkout-dialog purchase-delete-confirm" role="alertdialog" aria-modal="true" aria-labelledby="purchase-delete-title"><div class="delete-confirm-mark"><CircleAlert :size="22"/></div><h2 id="purchase-delete-title">Eliminar la compra #{{ purchasePendingDelete.id }}?</h2><p>Se quitara la compra y sus detalles. Esta accion no se puede deshacer.</p><div class="sale-delete-warning"><CircleAlert :size="15"/><span>El inventario se repone. Si fue una compra en efectivo, se registrara una salida compensatoria en la caja.</span></div><div v-if="deleteError" class="alert-box" role="alert">{{ deleteError }}</div><footer class="checkout-dialog-actions"><button class="btn btn-secondary" :disabled="deletingId !== null" @click="cancelDelete">Cancelar</button><button class="btn btn-danger" :disabled="deletingId !== null" @click="deletePurchase">{{ deletingId !== null ? 'Eliminando...' : 'Eliminar compra' }}</button></footer></section></div>
  </section>
</template>