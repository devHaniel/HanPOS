<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { saleService } from '@/Service'
import type { PagedResult } from '@/Type/Common'
import type { Sale } from '@/Type/Sale'
const result = ref<PagedResult<Sale> | null>(null)
const loading = ref(false)
const error = ref('')
const currency = (amount: number) => new Intl.NumberFormat('es-HN',{style:'currency',currency:'HNL'}).format(amount)
const shortDate = (value: string) => new Intl.DateTimeFormat('es-HN',{dateStyle:'medium',timeStyle:'short'}).format(new Date(value))
async function load() { loading.value=true; error.value=''; try { result.value=await saleService.list({pagina:1,cantidad:50}) } catch(cause) { error.value=cause instanceof Error?cause.message:'No se pudieron cargar las ventas.' } finally { loading.value=false } }
onMounted(load)
</script>
<template><section class="fade-up"><div class="page-heading"><div><p class="eyebrow">Historial</p><h1>Ventas</h1><p>Consulta transacciones registradas y sus totales.</p></div><RouterLink class="btn" to="/pos">Nueva venta <span aria-hidden="true">→</span></RouterLink></div>
  <div v-if="error" class="alert-box" style="margin-bottom:14px">{{ error }}</div><section class="panel"><div class="panel-header"><div><h2>Transacciones</h2><p>{{ result?.total ?? 0 }} ventas registradas</p></div><button class="btn btn-secondary" @click="load">Actualizar</button></div><div v-if="loading" class="loading-state">Cargando ventas…</div><div v-else class="table-wrap"><table class="data-table"><thead><tr><th>Número</th><th>Fecha</th><th>Cliente</th><th>Caja</th><th>Estado</th><th>Total</th></tr></thead><tbody><tr v-for="sale in result?.items ?? []" :key="sale.id"><td class="row-primary">#{{ sale.id }}</td><td>{{ shortDate(sale.fecha) }}</td><td>{{ sale.clienteId ?? 'Consumidor final' }}</td><td>#{{ sale.cajaId }}</td><td><span class="status-pill">{{ sale.estado === 1 ? 'Completada' : 'Registrada' }}</span></td><td class="row-primary">{{ currency(sale.total) }}</td></tr></tbody></table><div v-if="!result?.items.length" class="empty-state">Todavía no hay ventas.</div></div></section></section></template>
