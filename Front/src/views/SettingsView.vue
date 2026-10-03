<script setup lang="ts">
import { ref } from 'vue'
import { ArrowRight, Bell, Building2, Check, Component, Store } from '@lucide/vue'
const storeName = ref('Mi tienda')
const ticketFooter = ref('Gracias por tu compra')
const saved = ref(false)
function savePreferences() { saved.value = true }
</script>
<template>
  <section class="fade-up settings-page">
    <div class="page-heading"><div><p class="eyebrow">Preferencias</p><h1>Configuración</h1><p>Datos básicos del punto de venta.</p></div></div>
    <div class="settings-grid">
      <section class="panel"><div class="panel-header"><div class="settings-title"><span class="settings-icon"><Store :size="17"/></span><div><h2>Información de la tienda</h2><p>Datos que aparecen en el sistema.</p></div></div></div>
        <form class="settings-form" @submit.prevent="savePreferences"><label class="form-field">Nombre comercial<input v-model="storeName" class="field" /></label><label class="form-field">Mensaje al pie del comprobante<input v-model="ticketFooter" class="field" /></label><button class="btn" type="submit">Guardar cambios</button><span v-if="saved" class="settings-saved"><Check :size="14"/> Cambios listos</span></form>
      </section>
      <section class="panel"><div class="panel-header"><div class="settings-title"><span class="settings-icon"><Bell :size="17"/></span><div><h2>Preferencias de pantalla</h2><p>Controles visuales de la terminal.</p></div></div></div>
        <div class="settings-options"><label class="settings-option"><span><strong>Confirmar antes de quitar productos</strong><small>Evita retirar líneas del carrito por accidente.</small></span><input type="checkbox" checked /></label><label class="settings-option"><span><strong>Mostrar existencias en el catálogo</strong><small>Consulta el stock disponible al vender.</small></span><input type="checkbox" checked /></label></div>
      </section>
      <RouterLink class="settings-library-card" to="/component-library"><span class="settings-icon"><Component :size="18"/></span><span><strong>Biblioteca de componentes</strong><small>Botones, campos, estados y piezas del POS</small></span><ArrowRight :size="17"/></RouterLink>
    </div>
  </section>
</template>
