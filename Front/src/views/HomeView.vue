<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { saleService } from '@/Service'
import { useProductStore } from '@/Store'
import type { Sale } from '@/Type/Sale'

const products = useProductStore()
const recentSales = ref<Sale[]>([])
const salesTotal = ref(0)
const loadError = ref('')
const currency = (amount: number) => new Intl.NumberFormat('es-HN', { style: 'currency', currency: 'HNL' }).format(amount)
const shortDate = (value: string) => new Intl.DateTimeFormat('es-HN', { day: '2-digit', month: 'short' }).format(new Date(value))

onMounted(async () => {
  const results = await Promise.allSettled([
    products.fetchAll({ pagina: 1, cantidad: 10 }),
    saleService.list({ pagina: 1, cantidad: 5 }),
  ])
  const salesResult = results[1]
  if (salesResult.status === 'fulfilled') {
    recentSales.value = salesResult.value.items
    salesTotal.value = salesResult.value.total
  }
  if (results.some((result) => result.status === 'rejected')) loadError.value = 'No fue posible cargar todos los datos. Comprueba tu sesión y conexión.'
})
</script>

<template>
  <section class="fade-up">
    <div class="page-heading"><div><p class="eyebrow">Operación diaria</p><h1>Resumen del negocio</h1><p>Estado actual de ventas e inventario.</p></div><RouterLink class="btn" to="/pos">Abrir punto de venta <span aria-hidden="true">→</span></RouterLink></div>
    <div v-if="loadError" class="alert-box" style="margin-bottom: 16px">{{ loadError }}</div>
    <div class="metric-grid">
      <article class="metric-card"><div class="metric-top"><span>Ventas registradas</span><span class="metric-mark">VT</span></div><div class="metric-value">{{ salesTotal }}</div><div class="metric-note">Transacciones registradas</div></article>
      <article class="metric-card"><div class="metric-top"><span>Productos activos</span><span class="metric-mark">PR</span></div><div class="metric-value">{{ products.total }}</div><div class="metric-note">Catálogo para venta</div></article>
      <article class="metric-card"><div class="metric-top"><span>Inventario bajo</span><span class="metric-mark">ST</span></div><div class="metric-value">{{ products.items.filter((product) => product.stock <= product.stockMinimo).length }}</div><div class="metric-note">Al nivel mínimo o por debajo</div></article>
      <article class="metric-card" style="background: #eaf2df"><div class="metric-top"><span>Turno de caja</span><span class="metric-mark">CJ</span></div><div class="metric-value" style="font-size: 18px">Operación</div><div class="metric-note"><RouterLink to="/cash-register">Revisar caja →</RouterLink></div></article>
    </div>
    <div class="dashboard-grid">
      <section class="panel"><div class="panel-header"><div><h2>Ventas recientes</h2><p>Últimos movimientos registrados</p></div><RouterLink class="table-action" to="/sales">Ver historial →</RouterLink></div>
        <div class="table-wrap"><table class="data-table"><thead><tr><th>Venta</th><th>Fecha</th><th>Cliente</th><th>Total</th></tr></thead><tbody>
          <tr v-for="sale in recentSales" :key="sale.id"><td class="row-primary">#{{ sale.id }}</td><td>{{ shortDate(sale.fecha) }}</td><td>{{ sale.clienteId ?? 'Consumidor final' }}</td><td class="row-primary">{{ currency(sale.total) }}</td></tr>
        </tbody></table><div v-if="!recentSales.length && !loadError" class="empty-state">Aún no hay ventas registradas.</div></div>
      </section>
      <section class="panel"><div class="panel-header"><div><h2>Accesos rápidos</h2><p>Acciones frecuentes</p></div></div><div class="panel-body quick-actions">
        <RouterLink class="quick-action" to="/pos"><span><strong>Nueva venta</strong><small>Armar pedido y cobrar</small></span><span class="quick-arrow">→</span></RouterLink>
        <RouterLink class="quick-action" to="/products"><span><strong>Inventario</strong><small>Consultar productos</small></span><span class="quick-arrow">→</span></RouterLink>
        <RouterLink class="quick-action" to="/purchases"><span><strong>Compras</strong><small>Revisar abastecimiento</small></span><span class="quick-arrow">→</span></RouterLink>
        <RouterLink class="quick-action" to="/cash-register"><span><strong>Control de caja</strong><small>Turno y movimientos</small></span><span class="quick-arrow">→</span></RouterLink>
      </div></section>
    </div>
  </section>
</template>
