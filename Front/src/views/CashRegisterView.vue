<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { Archive, Clock3, Eye, LockKeyhole, RefreshCw, Wallet, X } from '@lucide/vue'
import { cashRegisterService } from '@/Service'
import { useAuthStore } from '@/Store'
import type { PagedResult } from '@/Type/Common'
import type { CashMovement } from '@/Type/CashMovement'
import type { CashRegister } from '@/Type/CashRegister'

const auth = useAuthStore()
const registers = ref<CashRegister[]>([])
const currentRegister = ref<CashRegister | null>(null)
const selectedRegister = ref<CashRegister | null>(null)
const selectedMovements = ref<CashMovement[]>([])
const openingAmount = ref(0)
const loading = ref(false)
const detailsLoading = ref(false)
const busy = ref(false)
const error = ref('')
const detailsError = ref('')
const notice = ref('')
const openCount = computed(() => registers.value.filter((item) => item.estaAbierta).length)
const closedCount = computed(() => registers.value.length - openCount.value)
const currentFloat = computed(() => registers.value.filter((item) => item.estaAbierta).reduce((sum, item) => sum + item.montoInicial, 0))
const currency = (amount: number) => new Intl.NumberFormat('es-HN', { style: 'currency', currency: 'HNL' }).format(amount)
const dateTime = (value: string | null) => value ? new Intl.DateTimeFormat('es-HN', { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(value)) : '—'

async function load() {
  loading.value = true
  error.value = ''
  const [history, open] = await Promise.allSettled([
    cashRegisterService.list({ pagina: 1, cantidad: 100 }),
    cashRegisterService.getOpen(),
  ])
  if (history.status === 'fulfilled') registers.value = history.value.items
  else error.value = history.reason instanceof Error ? history.reason.message : 'No se pudo cargar el historial.'
  currentRegister.value = open.status === 'fulfilled' ? open.value : null
  if (open.status === 'rejected' && 'status' in open.reason && open.reason.status !== 404) {
    error.value = open.reason instanceof Error ? open.reason.message : 'No se pudo consultar la caja abierta.'
  }
  loading.value = false
}

async function openRegister() {
  if (!auth.user?.id) return
  busy.value = true
  error.value = ''
  try {
    await cashRegisterService.create({ usuarioId: auth.user.id, montoInicial: openingAmount.value })
    openingAmount.value = 0
    notice.value = 'Caja abierta correctamente.'
    await load()
  } catch (cause) {
    error.value = cause instanceof Error ? cause.message : 'No se pudo abrir la caja.'
  } finally { busy.value = false }
}

async function closeRegister() {
  if (!currentRegister.value || !window.confirm('¿Deseas cerrar la caja actual?')) return
  busy.value = true
  error.value = ''
  try {
    await cashRegisterService.close(currentRegister.value.id)
    notice.value = 'Caja cerrada correctamente.'
    await load()
  } catch (cause) {
    error.value = cause instanceof Error ? cause.message : 'No se pudo cerrar la caja.'
  } finally { busy.value = false }
}

async function showDetails(register: CashRegister) {
  selectedRegister.value = register
  selectedMovements.value = []
  detailsError.value = ''
  detailsLoading.value = true
  try {
    selectedMovements.value = await cashRegisterService.getMovements(register.id)
  } catch (cause) {
    detailsError.value = cause instanceof Error ? cause.message : 'No se pudieron cargar los movimientos.'
  } finally {
    detailsLoading.value = false
  }
}

function closeDetails() {
  selectedRegister.value = null
  selectedMovements.value = []
  detailsError.value = ''
}

onMounted(load)
</script>
<template>
  <section class="fade-up register-page">
    <div class="page-heading"><div><p class="eyebrow">Control de efectivo</p><h1>Caja</h1><p>Turnos de caja y fondos de apertura.</p></div><button class="btn btn-secondary" :disabled="loading" @click="load"><RefreshCw :size="15"/> Actualizar</button></div>
    <div v-if="error" class="alert-box register-alert">{{ error }}</div><div v-if="notice" class="register-notice">{{ notice }}</div>

    <div class="metric-grid register-metrics">
      <article class="metric-card"><div class="metric-top"><span>Turnos registrados</span><span class="metric-mark"><Archive :size="15"/></span></div><div class="metric-value">{{ registers.length }}</div><div class="metric-note">Historial de cajas</div></article>
      <article class="metric-card"><div class="metric-top"><span>Caja abierta</span><span class="metric-mark"><LockKeyhole :size="15"/></span></div><div class="metric-value">{{ openCount }}</div><div class="metric-note">Turnos en curso</div></article>
      <article class="metric-card"><div class="metric-top"><span>Cajas cerradas</span><span class="metric-mark"><Clock3 :size="15"/></span></div><div class="metric-value">{{ closedCount }}</div><div class="metric-note">Turnos completados</div></article>
      <article class="metric-card register-float-card"><div class="metric-top"><span>Fondo inicial activo</span><span class="metric-mark"><Wallet :size="15"/></span></div><div class="metric-value register-money">{{ currency(currentFloat) }}</div><div class="metric-note">Total de fondos en cajas abiertas</div></article>
    </div>

    <div class="register-content-grid">
      <section class="panel"><div class="panel-header"><div><h2>Historial de cajas</h2><p>{{ registers.length }} turnos encontrados</p></div><span class="register-history-mark"><Archive :size="17"/></span></div>
        <div v-if="loading" class="loading-state">Cargando historial…</div>
        <div v-else class="table-wrap"><table class="data-table"><thead><tr><th>Apertura</th><th>Cierre</th><th>Usuario</th><th>Fondo inicial</th><th>Fondo final</th><th>Estado</th><th>Detalle</th></tr></thead><tbody>
          <tr v-for="item in registers" :key="item.id"><td><strong class="table-main-text">{{ dateTime(item.fechaApertura) }}</strong><small class="table-subtext">Caja #{{ item.id }}</small></td><td>{{ dateTime(item.fechaCierre) }}</td><td>Usuario #{{ item.usuarioId }}</td><td class="register-amount">{{ currency(item.montoInicial) }}</td><td>{{ item.montoFinal === null ? '—' : currency(item.montoFinal) }}</td><td><span class="status-pill" :class="{closed: !item.estaAbierta}">{{ item.estaAbierta ? 'Abierta' : 'Cerrada' }}</span></td><td><button class="table-action register-detail-button" :aria-label="`Ver detalles de caja ${item.id}`" @click="showDetails(item)"><Eye :size="15"/><span>Ver</span></button></td></tr>
        </tbody></table><div v-if="!registers.length" class="empty-state"><Archive :size="25"/><strong>Aún no hay cajas</strong><span>Al abrir un turno, aparecerá en este historial.</span></div></div>
      </section>
      <aside class="panel register-action-panel"><div class="panel-header"><div><h2>{{ currentRegister ? 'Turno actual' : 'Abrir caja' }}</h2><p>{{ currentRegister ? `Caja #${currentRegister.id}` : 'Inicia un nuevo turno' }}</p></div><span class="register-history-mark"><Wallet :size="17"/></span></div>
        <div class="register-action-body"><template v-if="currentRegister"><div class="register-open-state"><span class="status-indicator"></span><strong>Caja abierta</strong><small>Desde {{ dateTime(currentRegister.fechaApertura) }}</small></div><div class="register-open-amount"><span>Fondo inicial</span><strong>{{ currency(currentRegister.montoInicial) }}</strong></div><RouterLink class="register-movement-link" to="/movements">Ver movimientos <ArrowRight :size="15"/></RouterLink><button class="btn btn-danger register-close-button" :disabled="busy" @click="closeRegister">{{ busy ? 'Cerrando…' : 'Cerrar turno de caja' }}</button></template>
          <form v-else class="register-open-form" @submit.prevent="openRegister"><span class="register-open-illustration"><Wallet :size="22"/></span><p>Registra el fondo inicial para comenzar a operar.</p><label class="form-field">Monto inicial<input v-model.number="openingAmount" class="field" type="number" min="0" step="0.01" required placeholder="0.00"/></label><button class="btn" type="submit" :disabled="busy">{{ busy ? 'Abriendo…' : 'Abrir caja' }}</button></form>
        </div>
      </aside>
    </div>

    <div v-if="selectedRegister" class="dialog-backdrop" @click.self="closeDetails">
      <section class="checkout-dialog register-details-dialog" role="dialog" aria-modal="true" :aria-labelledby="'register-details-title'">
        <header class="checkout-dialog-header"><div><span class="dialog-step-label">HISTORIAL DE CAJA</span><h2 id="register-details-title">Detalle de caja #{{ selectedRegister.id }}</h2><p>Apertura {{ dateTime(selectedRegister.fechaApertura) }}</p></div><button class="dialog-close" aria-label="Cerrar detalle" @click="closeDetails"><X :size="18"/></button></header>
        <div class="register-details-summary">
          <div><span>Estado</span><strong><span class="status-pill" :class="{closed: !selectedRegister.estaAbierta}">{{ selectedRegister.estaAbierta ? 'Abierta' : 'Cerrada' }}</span></strong></div>
          <div><span>Usuario</span><strong>Usuario #{{ selectedRegister.usuarioId }}</strong></div>
          <div><span>Fondo inicial</span><strong>{{ currency(selectedRegister.montoInicial) }}</strong></div>
          <div><span>Fondo final</span><strong>{{ selectedRegister.montoFinal === null ? '—' : currency(selectedRegister.montoFinal) }}</strong></div>
          <div class="register-detail-close-time"><span>Fecha de cierre</span><strong>{{ dateTime(selectedRegister.fechaCierre) }}</strong></div>
        </div>
        <div class="register-details-history"><div class="register-details-history-heading"><div><h3>Movimientos</h3><p>{{ selectedMovements.length }} registros asociados</p></div></div>
          <div v-if="detailsLoading" class="loading-state">Cargando detalle…</div>
          <div v-else-if="detailsError" class="alert-box">{{ detailsError }}</div>
          <div v-else class="table-wrap"><table class="data-table"><thead><tr><th>Fecha</th><th>Tipo</th><th>Descripción</th><th>Monto</th></tr></thead><tbody><tr v-for="movement in selectedMovements" :key="movement.id"><td>{{ dateTime(movement.fecha) }}</td><td><span class="status-pill" :class="{alert: movement.tipoMovimiento === 2}">{{ movement.tipoMovimiento === 1 ? 'Entrada' : 'Salida' }}</span></td><td>{{ movement.concepto || 'Sin descripción' }}</td><td class="register-amount">{{ currency(movement.monto) }}</td></tr></tbody></table><div v-if="!selectedMovements.length" class="empty-state"><Wallet :size="22"/><strong>Sin movimientos</strong><span>Esta caja no tiene movimientos registrados.</span></div></div>
        </div>
        <footer class="checkout-dialog-actions"><button class="btn btn-secondary" @click="closeDetails">Cerrar</button></footer>
      </section>
    </div>
  </section>
</template>
