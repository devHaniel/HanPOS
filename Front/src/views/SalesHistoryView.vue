<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { CircleAlert, Eye, FileText, RefreshCw, Trash2, X } from '@lucide/vue'
import { productService, saleService } from '@/Service'
import { useAuthStore } from '@/Store'
import type { PagedResult } from '@/Type/Common'
import type { Sale, SaleDetail } from '@/Type/Sale'

interface SaleDetailRow extends SaleDetail {
  productName: string
  productCode?: string
}

const auth = useAuthStore()
const result = ref<PagedResult<Sale> | null>(null)
const selectedSale = ref<Sale | null>(null)
const saleDetails = ref<SaleDetailRow[]>([])
const salePendingDelete = ref<Sale | null>(null)
const loading = ref(false)
const detailsLoading = ref(false)
const deletingId = ref<number | null>(null)
const error = ref('')
const notice = ref('')
const detailsError = ref('')
const deleteError = ref('')
const currency = (amount: number) => new Intl.NumberFormat('es-HN', { style: 'currency', currency: 'HNL' }).format(amount)
const shortDate = (value: string) => new Intl.DateTimeFormat('es-HN', { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(value))

function paymentMethod(method: number | null) {
  if (method === 1) return 'Efectivo'
  if (method === 2) return 'Tarjeta'
  if (method === 3) return 'Transferencia'
  return 'No especificado'
}

async function load() {
  loading.value = true
  error.value = ''
  try {
    result.value = await saleService.list({ pagina: 1, cantidad: 50 })
  } catch (cause) {
    error.value = cause instanceof Error ? cause.message : 'No se pudieron cargar las ventas.'
  } finally {
    loading.value = false
  }
}

async function showDetails(sale: Sale) {
  selectedSale.value = sale
  saleDetails.value = []
  detailsError.value = ''
  detailsLoading.value = true
  try {
    const details = await saleService.getDetails(sale.id)
    saleDetails.value = await Promise.all(details.map(async (detail) => {
      try {
        const product = await productService.getById(detail.productoId)
        return { ...detail, productName: product.nombre, productCode: product.codigo }
      } catch {
        return { ...detail, productName: `Producto #${detail.productoId}` }
      }
    }))
  } catch (cause) {
    detailsError.value = cause instanceof Error ? cause.message : 'No se pudieron cargar los detalles.'
  } finally {
    detailsLoading.value = false
  }
}

function closeDetails() {
  selectedSale.value = null
  saleDetails.value = []
  detailsError.value = ''
}

function requestDelete(sale: Sale) {
  if (!auth.isAdmin) return
  deleteError.value = ''
  salePendingDelete.value = sale
}

function cancelDelete() {
  if (deletingId.value !== null) return
  salePendingDelete.value = null
  deleteError.value = ''
}

async function deleteSale() {
  const sale = salePendingDelete.value
  if (!auth.isAdmin || !sale) return
  deletingId.value = sale.id
  deleteError.value = ''
  error.value = ''
  try {
    await saleService.remove(sale.id)
    notice.value = `La venta #${sale.id} fue eliminada.`
    salePendingDelete.value = null
    if (selectedSale.value?.id === sale.id) closeDetails()
    await load()
  } catch (cause) {
    deleteError.value = cause instanceof Error ? cause.message : 'No se pudo eliminar la venta.'
  } finally {
    deletingId.value = null
  }
}

onMounted(load)
</script>

<template>
  <section class="fade-up sales-history-page">
    <div class="page-heading">
      <div><p class="eyebrow">Historial</p><h1>Ventas</h1><p>Consulta operaciones y revisa sus productos.</p></div>
      <RouterLink class="btn" to="/pos">Nueva venta <span aria-hidden="true">→</span></RouterLink>
    </div>
    <div v-if="error" class="alert-box sales-alert" role="alert">{{ error }}</div>
    <div v-if="notice" class="register-notice">{{ notice }}</div>
    <section class="panel">
      <div class="panel-header"><div><h2>Transacciones</h2><p>{{ result?.total ?? 0 }} ventas registradas</p></div><button class="btn btn-secondary" :disabled="loading" @click="load"><RefreshCw :size="15"/> Actualizar</button></div>
      <div v-if="loading" class="loading-state">Cargando ventas…</div>
      <div v-else class="table-wrap"><table class="data-table"><thead><tr><th>Número</th><th>Fecha</th><th>Cliente</th><th>Caja</th><th>Pago</th><th>Estado</th><th>Total</th><th>Acciones</th></tr></thead><tbody>
        <tr v-for="sale in result?.items ?? []" :key="sale.id"><td class="row-primary">#{{ sale.id }}</td><td>{{ shortDate(sale.fecha) }}</td><td>{{ sale.clienteId ? `Cliente #${sale.clienteId}` : 'Cliente ocasional' }}</td><td>#{{ sale.cajaId }}</td><td>{{ paymentMethod(sale.metodoPago) }}</td><td><span class="status-pill" :class="{ alert: sale.estado === 2 }">{{ sale.estado === 2 ? 'Cancelada' : 'Completada' }}</span></td><td class="row-primary">{{ currency(sale.total) }}</td><td><div class="table-actions"><button class="table-action" :aria-label="`Ver venta ${sale.id}`" @click="showDetails(sale)"><Eye :size="15"/><span>Detalle</span></button><button v-if="auth.isAdmin" class="table-action delete" :aria-label="`Eliminar venta ${sale.id}`" :disabled="deletingId === sale.id" @click="requestDelete(sale)"><Trash2 :size="14"/><span>Eliminar</span></button></div></td></tr>
      </tbody></table><div v-if="!result?.items.length" class="empty-state"><FileText :size="24"/><strong>Aún no hay ventas</strong><span>Las ventas realizadas aparecerán en este historial.</span></div></div>
    </section>

    <div v-if="selectedSale" class="dialog-backdrop" @click.self="closeDetails"><section class="checkout-dialog sale-details-dialog" role="dialog" aria-modal="true" aria-labelledby="'sale-details-title'">
      <header class="checkout-dialog-header"><div><span class="dialog-step-label">DETALLE DE OPERACIÓN</span><h2 id="sale-details-title">Venta #{{ selectedSale.id }}</h2><p>{{ shortDate(selectedSale.fecha) }} · {{ paymentMethod(selectedSale.metodoPago) }}</p></div><button class="dialog-close" aria-label="Cerrar detalle" @click="closeDetails"><X :size="18"/></button></header>
      <div class="sale-details-meta"><div><span>Cliente</span><strong>{{ selectedSale.clienteId ? `Cliente #${selectedSale.clienteId}` : 'Cliente ocasional' }}</strong></div><div><span>Caja</span><strong>#{{ selectedSale.cajaId }}</strong></div><div><span>Usuario</span><strong>#{{ selectedSale.usuarioId }}</strong></div><div><span>Estado</span><strong>{{ selectedSale.estado === 2 ? 'Cancelada' : 'Completada' }}</strong></div></div>
      <div class="sale-details-content"><div class="sale-details-section-title"><div><h3>Productos</h3><p>{{ saleDetails.length }} líneas en esta venta</p></div></div>
        <div v-if="detailsLoading" class="loading-state">Cargando productos…</div><div v-else-if="detailsError" class="alert-box">{{ detailsError }}</div>
        <div v-else class="table-wrap"><table class="data-table"><thead><tr><th>Producto</th><th>Cantidad</th><th>Precio unitario</th><th>Importe</th></tr></thead><tbody><tr v-for="line in saleDetails" :key="line.id"><td><strong class="table-main-text">{{ line.productName }}</strong><small v-if="line.productCode" class="table-subtext">{{ line.productCode }}</small></td><td>{{ line.cantidad }}</td><td>{{ currency(line.precioUnitario) }}</td><td class="register-amount">{{ currency(line.subtotal) }}</td></tr></tbody></table><div v-if="!saleDetails.length" class="empty-state">Esta venta no tiene líneas de detalle.</div></div>
      </div>
      <div class="sale-details-totals"><div><span>Subtotal</span><strong>{{ currency(selectedSale.subtotal) }}</strong></div><div><span>Impuesto</span><strong>{{ currency(selectedSale.impuesto) }}</strong></div><div class="sale-details-total"><span>Total</span><strong>{{ currency(selectedSale.total) }}</strong></div></div>
      <footer class="checkout-dialog-actions"><button v-if="auth.isAdmin" class="btn btn-danger" :disabled="deletingId === selectedSale.id" @click="requestDelete(selectedSale)"><Trash2 :size="14"/> Eliminar venta</button><button class="btn btn-secondary" @click="closeDetails">Cerrar</button></footer>
    </section></div>

    <div v-if="salePendingDelete" class="dialog-backdrop confirm-delete-backdrop" @click.self="cancelDelete"><section class="checkout-dialog sale-delete-confirm" role="alertdialog" aria-modal="true" aria-labelledby="sale-delete-title"><div class="delete-confirm-mark"><CircleAlert :size="22"/></div><h2 id="sale-delete-title">¿Eliminar la venta #{{ salePendingDelete.id }}?</h2><p>Se quitarán la venta y sus detalles. Esta acción no se puede deshacer.</p><div class="sale-delete-warning"><CircleAlert :size="15"/><span>El inventario se repondrá. Si fue una venta en efectivo, se registrará una salida compensatoria en la caja.</span></div><div v-if="deleteError" class="alert-box" role="alert">{{ deleteError }}</div><footer class="checkout-dialog-actions"><button class="btn btn-secondary" :disabled="deletingId !== null" @click="cancelDelete">Cancelar</button><button class="btn btn-danger" :disabled="deletingId !== null" @click="deleteSale">{{ deletingId !== null ? 'Eliminando…' : 'Eliminar venta' }}</button></footer></section></div>
  </section>
</template>
