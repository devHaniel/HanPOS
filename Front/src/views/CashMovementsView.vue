<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { ArrowLeft, ArrowLeftRight, ArrowUpRight, Plus, RefreshCw, Wallet } from '@lucide/vue'
import { cashMovementService, cashRegisterService } from '@/Service'
import { useAuthStore } from '@/Store'
import type { CashMovement } from '@/Type/CashMovement'
import type { CashRegister } from '@/Type/CashRegister'
const auth = useAuthStore()
const register = ref<CashRegister | null>(null)
const movements = ref<CashMovement[]>([])
const movementAmount = ref(0)
const movementConcept = ref('')
const movementKind = ref<1 | 2>(1)
const busy = ref(false)
const loading = ref(false)
const error = ref('')
const notice = ref('')
const currency = (amount: number) => new Intl.NumberFormat('es-HN', { style: 'currency', currency: 'HNL' }).format(amount)
const dateTime = (value: string) => new Intl.DateTimeFormat('es-HN', { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(value))
const entriesTotal = computed(() => movements.value.filter((item) => item.tipoMovimiento === 1).reduce((sum, item) => sum + item.monto, 0))
const exitsTotal = computed(() => movements.value.filter((item) => item.tipoMovimiento === 2).reduce((sum, item) => sum + item.monto, 0))
async function load() {
  loading.value = true
  error.value = ''
  try {
    register.value = await cashRegisterService.getOpen()
    movements.value = await cashMovementService.getByRegister(register.value.id)
  } catch (cause) {
    register.value = null
    movements.value = []
    if (cause instanceof Error && (!('status' in cause) || cause.status !== 404)) error.value = cause.message
  } finally { loading.value = false }
}
async function addMovement() {
  if (!register.value || movementAmount.value <= 0) return
  busy.value = true
  error.value = ''
  try {
    await cashMovementService.create({ cajaId: register.value.id, monto: movementAmount.value, tipoMovimiento: movementKind.value, concepto: movementConcept.value.trim() || null })
    movementAmount.value = 0
    movementConcept.value = ''
    notice.value = 'Movimiento guardado.'
    await load()
  } catch (cause) { error.value = cause instanceof Error ? cause.message : 'No se pudo guardar el movimiento.' }
  finally { busy.value = false }
}
onMounted(load)
</script>
<template>
  <section class="fade-up movements-page">
    <div class="page-heading"><div><p class="eyebrow">Control de caja</p><h1>Movimientos</h1><p>Entradas y retiros registrados en el turno activo.</p></div><button class="btn btn-secondary" :disabled="loading" @click="load"><RefreshCw :size="15"/> Actualizar</button></div>
    <div v-if="error" class="alert-box" style="margin-bottom:14px">{{ error }}</div><div v-if="notice" class="register-notice">{{ notice }}</div>
    <div v-if="!register" class="movement-no-register panel"><span class="register-open-illustration"><Wallet :size="22"/></span><h2>No hay una caja abierta</h2><p>Abre un turno para registrar y consultar movimientos.</p><RouterLink class="btn" to="/cash-register"><ArrowLeft :size="15"/> Ir a Caja</RouterLink></div>
    <template v-else>
      <div class="movement-summary-grid"><article class="metric-card"><div class="metric-top"><span>Entradas</span><span class="metric-mark movement-entry"><ArrowUpRight :size="15"/></span></div><div class="metric-value">{{ currency(entriesTotal) }}</div><div class="metric-note">{{ movements.filter((item) => item.tipoMovimiento === 1).length }} registros</div></article><article class="metric-card"><div class="metric-top"><span>Salidas</span><span class="metric-mark movement-exit"><ArrowLeftRight :size="15"/></span></div><div class="metric-value">{{ currency(exitsTotal) }}</div><div class="metric-note">{{ movements.filter((item) => item.tipoMovimiento === 2).length }} registros</div></article><article class="metric-card movement-register-card"><div class="metric-top"><span>Turno activo</span><span class="metric-mark"><Wallet :size="15"/></span></div><div class="metric-value">Caja #{{ register.id }}</div><div class="metric-note">Abierta {{ dateTime(register.fechaApertura) }}</div></article></div>
      <div class="movement-layout"><section class="panel"><div class="panel-header"><div><h2>Historial del turno</h2><p>{{ movements.length }} movimientos registrados</p></div><span class="register-history-mark"><ArrowLeftRight :size="17"/></span></div><div v-if="loading" class="loading-state">Cargando movimientos…</div><div v-else class="table-wrap"><table class="data-table"><thead><tr><th>Fecha</th><th>Tipo</th><th>Descripción</th><th>Monto</th><th>Usuario</th><th>Estado</th></tr></thead><tbody><tr v-for="movement in movements" :key="movement.id"><td>{{ dateTime(movement.fecha) }}</td><td><span class="status-pill" :class="{alert: movement.tipoMovimiento === 2}">{{ movement.tipoMovimiento === 1 ? 'Entrada' : 'Salida' }}</span></td><td>{{ movement.concepto || 'Sin descripción' }}</td><td class="register-amount">{{ currency(movement.monto) }}</td><td>{{ auth.user?.nombre || auth.user?.username || `Usuario #${register.usuarioId}` }}</td><td><span class="status-pill">Registrado</span></td></tr></tbody></table><div v-if="!movements.length" class="empty-state"><ArrowLeftRight :size="24"/><strong>Sin movimientos</strong><span>Los registros de este turno aparecerán aquí.</span></div></div></section>
        <aside class="panel movement-form-panel"><div class="panel-header"><div><h2>Nuevo movimiento</h2><p>Registra una entrada o salida.</p></div><span class="register-history-mark"><Plus :size="17"/></span></div><form class="movement-form" @submit.prevent="addMovement"><label class="form-field">Tipo<select v-model.number="movementKind" class="field"><option :value="1">Entrada de efectivo</option><option :value="2">Salida de efectivo</option></select></label><label class="form-field">Descripción<input v-model="movementConcept" class="field" maxlength="200" placeholder="Ej. Cambio para caja"/></label><label class="form-field">Monto<input v-model.number="movementAmount" class="field" type="number" min="0.01" step="0.01" inputmode="decimal" placeholder="0.00" required/></label><button class="btn" type="submit" :disabled="busy"><Plus :size="15"/>{{ busy ? 'Guardando…' : 'Guardar movimiento' }}</button></form></aside>
      </div>
    </template>
  </section>
</template>
+