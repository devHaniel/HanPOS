<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { cashRegisterService, productService, providerService, purchaseService } from '@/Service'
import { useAuthStore } from '@/Store'
import type { PagedResult } from '@/Type/Common'
import type { Product } from '@/Type/Product'
import type { Purchase } from '@/Type/Purchase'
import type { Provider } from '@/Type/Provider'
const auth=useAuthStore()
const result=ref<PagedResult<Purchase>|null>(null)
const providers=ref<Provider[]>([])
const products=ref<Product[]>([])
const providerId=ref<number|null>(null)
const productId=ref<number|null>(null)
const quantity=ref(1)
const loading=ref(false)
const saving=ref(false)
const error=ref('')
const notice=ref('')
const currency=(amount:number)=>new Intl.NumberFormat('es-HN',{style:'currency',currency:'HNL'}).format(amount)
const shortDate=(value:string)=>new Intl.DateTimeFormat('es-HN',{dateStyle:'medium'}).format(new Date(value))
async function load(){loading.value=true;try{const results=await Promise.allSettled([purchaseService.list({pagina:1,cantidad:50}),providerService.list(),productService.list({pagina:1,cantidad:100})]);if(results[0].status==='fulfilled')result.value=results[0].value;if(results[1].status==='fulfilled')providers.value=results[1].value;if(results[2].status==='fulfilled')products.value=results[2].value.items}catch(cause){error.value=cause instanceof Error?cause.message:'No se pudieron cargar las compras.'}finally{loading.value=false}}
async function createPurchase(){if(!auth.user?.id||!providerId.value||!productId.value)return;saving.value=true;error.value='';try{const register=await cashRegisterService.getOpen();await purchaseService.create({usuarioId:auth.user.id,cajaId:register.id,proveedorId:providerId.value,detalles:[{productoId:productId.value,cantidad:quantity.value}]});notice.value='Compra registrada y existencia actualizada.';productId.value=null;providerId.value=null;quantity.value=1;await load()}catch(cause){error.value=cause instanceof Error?cause.message:'No se pudo registrar la compra.'}finally{saving.value=false}}
onMounted(load)
</script>
<template><section class="fade-up"><div class="page-heading"><div><p class="eyebrow">Abastecimiento</p><h1>Compras</h1><p>Registra entradas de mercancía vinculadas a un proveedor y turno de caja.</p></div></div>
  <div v-if="error" class="alert-box" style="margin-bottom:12px">{{ error }}</div><div v-if="notice" class="status-pill" style="margin-bottom:12px">{{ notice }}</div>
  <div class="dashboard-grid" style="margin-bottom:16px"><section class="panel"><div class="panel-header"><div><h2>Registrar compra</h2><p>La API actual registra una línea por operación.</p></div></div><form class="panel-body" @submit.prevent="createPurchase"><label class="form-field">Proveedor<select v-model.number="providerId" class="field" required><option :value="null" disabled>Seleccionar proveedor</option><option v-for="provider in providers" :key="provider.id" :value="provider.id">{{ provider.nombre }}</option></select></label><label class="form-field" style="margin-top:12px">Producto<select v-model.number="productId" class="field" required><option :value="null" disabled>Seleccionar producto</option><option v-for="product in products" :key="product.id" :value="product.id">{{ product.codigo }} · {{ product.nombre }}</option></select></label><label class="form-field" style="margin-top:12px">Cantidad<input v-model.number="quantity" class="field" type="number" min="0.01" step="any" required /></label><button class="btn" style="width:100%;margin-top:14px" :disabled="saving">{{ saving?'Registrando…':'Confirmar compra' }}</button></form></section><section class="panel"><div class="panel-header"><div><h2>Resumen</h2><p>Movimientos recibidos por la API</p></div><span class="metric-mark">CM</span></div><div class="panel-body"><div class="metric-value">{{ result?.total ?? 0 }}</div><div class="metric-note">Compras registradas</div><p class="metric-note">Para registrar una compra, la caja debe estar abierta.</p></div></section></div>
  <section class="panel"><div class="panel-header"><div><h2>Historial de compras</h2><p>{{ result?.total ?? 0 }} registros</p></div><button class="btn btn-secondary" @click="load">Actualizar</button></div><div v-if="loading" class="loading-state">Cargando compras…</div><div v-else class="table-wrap"><table class="data-table"><thead><tr><th>Compra</th><th>Fecha</th><th>Proveedor</th><th>Caja</th><th>Total</th></tr></thead><tbody><tr v-for="purchase in result?.items??[]" :key="purchase.id"><td class="row-primary">#{{ purchase.id }}</td><td>{{ shortDate(purchase.fecha) }}</td><td>#{{ purchase.proveedorId }}</td><td>#{{ purchase.cajaId }}</td><td class="row-primary">{{ currency(purchase.total) }}</td></tr></tbody></table><div v-if="!result?.items.length" class="empty-state">No hay compras registradas.</div></div></section></section></template>
