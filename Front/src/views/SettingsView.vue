<script setup lang="ts">
import { ref, onMounted, computed, reactive } from 'vue'
import { useNegocioStore } from '@/Store'
import { Store, Save, Loader2, AlertCircle, Check, Shield, Palette, Printer } from '@lucide/vue'

const negocioStore = useNegocioStore()

// Form state
const form = reactive({
  nombre: '',
  razonSocial: '',
  identificacionFiscal: '',
  direccion: '',
  telefono: '',
  email: '',
  mensajePieComprobante: '',
  logo: '',
  monedaDefecto: 'HN',
  simboloMoneda: 'Lps',
  zonaHoraria: 'America/Lima',
  activo: true
})

const preferenciasPantalla = ref<Record<string, any>>({})
const configuracionImpresion = ref<Record<string, any>>({})

const isLoading = computed(() => negocioStore.isLoading)
const error = computed(() => negocioStore.error)
const isNew = computed(() => !negocioStore.negocio)
const saved = ref(false)

async function cargarNegocio() {
  await negocioStore.fetch()
  if (negocioStore.negocio) {
    const n = negocioStore.negocio
    Object.assign(form, {
      nombre: n.nombre,
      razonSocial: n.razonSocial ?? '',
      identificacionFiscal: n.identificacionFiscal ?? '',
      direccion: n.direccion ?? '',
      telefono: n.telefono ?? '',
      email: n.email ?? '',
      mensajePieComprobante: n.mensajePieComprobante ?? '',
      logo: n.logo ?? '',
      monedaDefecto: n.monedaDefecto,
      simboloMoneda: n.simboloMoneda,
      zonaHoraria: n.zonaHoraria,
      activo: n.activo
    })
    preferenciasPantalla.value = { ...negocioStore.preferencias.value }
    configuracionImpresion.value = { ...negocioStore.configuracionImpresion.value }
  }
  // Set defaults if empty
  const defaultsPref = {
    compacto: false,
    mostrarStock: true,
    confirmarEliminacion: true,
    temaOscuro: false,
    sonidos: true
  }
  const defaultsPrint = {
    anchoPapel: 80,
    margenes: 5,
    tamanoFuente: 12,
    copias: 1,
    cortarPapel: true,
    abrirCajon: true
  }
  preferenciasPantalla.value = { ...defaultsPref, ...preferenciasPantalla.value }
  configuracionImpresion.value = { ...defaultsPrint, ...configuracionImpresion.value }
}

async function guardar() {
  negocioStore.clearError()
  saved.value = false
  const request = {
    ...form,
    preferenciasPantalla: JSON.stringify(preferenciasPantalla.value),
    configuracionImpresion: JSON.stringify(configuracionImpresion.value)
  }

  try {
    if (isNew.value) {
      await negocioStore.create(request)
    } else {
      await negocioStore.update(request)
    }
    await cargarNegocio()
    saved.value = true
    setTimeout(() => saved.value = false, 3000)
  } catch (err) {
    // Error handled by store
  }
}

onMounted(() => {
  cargarNegocio()
})
</script>

<template>
  <section class="fade-up settings-page">
    <div class="page-heading">
      <div>
        <p class="eyebrow">Preferencias</p>
        <h1>Configuración del Negocio</h1>
        <p>Datos fiscales, preferencias de pantalla e impresión. Este es el registro global del sistema.</p>
      </div>
    </div>

    <div v-if="error" class="alert-box" style="margin-bottom: 16px;">
      <AlertCircle :size="14" />
      {{ error }}
    </div>

    <div class="settings-grid">
      <!-- Información General -->
      <section class="panel">
        <div class="panel-header">
          <div class="settings-title">
            <span class="settings-icon"><Store :size="17" /></span>
            <div>
              <h2>Información del Negocio</h2>
              <p>Datos fiscales y de contacto que aparecen en comprobantes y reportes.</p>
            </div>
          </div>
        </div>
        <form class="settings-form" @submit.prevent="guardar">
          <div class="form-row">
            <label class="form-field">
              <span>Nombre Comercial <span class="required">*</span></span>
              <input v-model="form.nombre" class="field" placeholder="Mi Tienda" required />
            </label>
            <label class="form-field">
              <span>Razón Social</span>
              <input v-model="form.razonSocial" class="field" placeholder="Mi Tienda S.A.C." />
            </label>
          </div>

          <div class="form-row">
            <label class="form-field">
              <span>RUC / NIT <span class="required">*</span></span>
              <input v-model="form.identificacionFiscal" class="field" placeholder="20123456789" />
            </label>
            <label class="form-field">
              <span>Moneda</span>
              <select v-model="form.monedaDefecto" class="field">
                <option value="HN">HN - Lempiras (Lps)</option>
                <option value="USD">USD - Dólares ($)</option>
                <option value="EUR">EUR - Euros (€)</option>
              </select>
            </label>
          </div>

          <label class="form-field full-width">
            <span>Dirección Fiscal</span>
            <input v-model="form.direccion" class="field" placeholder="Av. Principal 123, Lima, Perú" />
          </label>

          <div class="form-row">
            <label class="form-field">
              <span>Teléfono</span>
              <input v-model="form.telefono" class="field" placeholder="+51 987 654 321" />
            </label>
            <label class="form-field">
              <span>Email</span>
              <input v-model="form.email" type="email" class="field" placeholder="contacto@mitienda.com" />
            </label>
          </div>

          <label class="form-field full-width">
            <span>Mensaje al Pie del Comprobante</span>
            <textarea v-model="form.mensajePieComprobante" class="field" rows="2" placeholder="¡Gracias por su compra! Vuelva pronto." />
          </label>

          <label class="form-field full-width">
            <span>Logo (Base64 o URL)</span>
            <input v-model="form.logo" class="field" placeholder="data:image/png;base64,..." />
            <small>Máximo 500KB. Se muestra en tickets y facturas.</small>
          </label>

          <div class="form-row">
            <label class="form-field">
              <span>Símbolo de Moneda</span>
              <input v-model="form.simboloMoneda" class="field" placeholder="S/" />
            </label>
            <label class="form-field">
              <span>Zona Horaria</span>
              <select v-model="form.zonaHoraria" class="field">
                <option value="America/Lima">America/Lima (UTC-5)</option>
                <option value="America/Mexico_City">America/Mexico_City (UTC-6)</option>
                <option value="America/Bogota">America/Bogota (UTC-5)</option>
                <option value="America/Santiago">America/Santiago (UTC-4/-3)</option>
                <option value="America/Argentina/Buenos_Aires">America/Argentina/Buenos_Aires (UTC-3)</option>
                <option value="Europe/Madrid">Europe/Madrid (UTC+1/+2)</option>
              </select>
            </label>
          </div>

          <label class="form-field checkbox-field full-width">
            <input type="checkbox" v-model="form.activo" />
            <span>Negocio Activo</span>
          </label>

          <div class="form-actions">
            <button class="btn btn-primary" type="submit" :disabled="isLoading">
              <Loader2 :size="16" class="animate-spin" v-if="isLoading" />
              <Save :size="16" v-else />
              <span>{{ isLoading ? 'Guardando...' : (isNew ? 'Crear Configuración Inicial' : 'Guardar Cambios') }}</span>
            </button>
            <span v-if="saved && !isLoading" class="settings-saved">
              <Check :size="14" /> Cambios guardados correctamente
            </span>
          </div>
        </form>
      </section>

      <!-- Preferencias de Pantalla -->
      <section class="panel">
        <div class="panel-header">
          <div class="settings-title">
            <span class="settings-icon"><Palette :size="17" /></span>
            <div>
              <h2>Preferencias de Pantalla</h2>
              <p>Configuración visual de la terminal de punto de venta.</p>
            </div>
          </div>
        </div>
        <div class="settings-options">
          <label class="settings-option">
            <span>
              <strong>Modo Compacto</strong>
              <small>Reduce el espaciado para mostrar más productos en pantalla.</small>
            </span>
            <input type="checkbox" v-model="preferenciasPantalla.compacto" @change="preferenciasPantalla = { ...preferenciasPantalla }" />
          </label>
          <label class="settings-option">
            <span>
              <strong>Mostrar Existencias</strong>
              <small>Muestra el stock disponible en el catálogo de productos.</small>
            </span>
            <input type="checkbox" v-model="preferenciasPantalla.mostrarStock" @change="preferenciasPantalla = { ...preferenciasPantalla }" />
          </label>
          <label class="settings-option">
            <span>
              <strong>Confirmar Eliminación</strong>
              <small>Pide confirmación antes de quitar productos del carrito.</small>
            </span>
            <input type="checkbox" v-model="preferenciasPantalla.confirmarEliminacion" @change="preferenciasPantalla = { ...preferenciasPantalla }" />
          </label>
          <label class="settings-option">
            <span>
              <strong>Tema Oscuro</strong>
              <small>Activa el modo oscuro en toda la interfaz.</small>
            </span>
            <input type="checkbox" v-model="preferenciasPantalla.temaOscuro" @change="preferenciasPantalla = { ...preferenciasPantalla }" />
          </label>
          <label class="settings-option">
            <span>
              <strong>Sonidos</strong>
              <small>Reproduce sonidos en acciones (venta completada, error, etc.).</small>
            </span>
            <input type="checkbox" v-model="preferenciasPantalla.sonidos" @change="preferenciasPantalla = { ...preferenciasPantalla }" />
          </label>
        </div>
      </section>

      <!-- Configuración de Impresión -->
      <section class="panel">
        <div class="panel-header">
          <div class="settings-title">
            <span class="settings-icon"><Printer :size="17" /></span>
            <div>
              <h2>Configuración de Impresión</h2>
              <p>Parámetros para tickets y facturas en impresoras térmicas.</p>
            </div>
          </div>
        </div>
        <div class="settings-form">
          <div class="form-row">
            <label class="form-field">
              <span>Ancho de Papel (mm)</span>
              <input v-model.number="configuracionImpresion.anchoPapel" type="number" class="field" min="58" max="216" @change="configuracionImpresion = { ...configuracionImpresion }" />
            </label>
            <label class="form-field">
              <span>Márgenes (mm)</span>
              <input v-model.number="configuracionImpresion.margenes" type="number" class="field" min="0" max="20" @change="configuracionImpresion = { ...configuracionImpresion }" />
            </label>
          </div>
          <div class="form-row">
            <label class="form-field">
              <span>Tamaño de Fuente</span>
              <input v-model.number="configuracionImpresion.tamanoFuente" type="number" class="field" min="8" max="18" @change="configuracionImpresion = { ...configuracionImpresion }" />
            </label>
            <label class="form-field">
              <span>Copias por Defecto</span>
              <input v-model.number="configuracionImpresion.copias" type="number" class="field" min="1" max="3" @change="configuracionImpresion = { ...configuracionImpresion }" />
            </label>
          </div>
          <label class="form-field checkbox-field">
            <input type="checkbox" v-model="configuracionImpresion.cortarPapel" @change="configuracionImpresion = { ...configuracionImpresion }" />
            <span>Cortar papel automáticamente</span>
          </label>
          <label class="form-field checkbox-field">
            <input type="checkbox" v-model="configuracionImpresion.abrirCajon" @change="configuracionImpresion = { ...configuracionImpresion }" />
            <span>Abrir cajón de dinero al imprimir</span>
          </label>
        </div>
      </section>
    </div>
  </section>
</template>

<style scoped>
.settings-page {
  max-width: 1000px;
  margin: 0 auto;
}

.settings-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(320px, 1fr));
  gap: 16px;
}

.panel {
  background: var(--paper);
  border: 1px solid var(--line);
  border-radius: 11px;
  overflow: hidden;
}

.panel-header {
  padding: 18px 20px;
  border-bottom: 1px solid var(--line);
  background: var(--surface-hover);
}

.settings-title {
  display: flex;
  align-items: flex-start;
  gap: 12px;
}

.settings-icon {
  color: var(--accent);
  flex-shrink: 0;
  margin-top: 2px;
  display: grid;
  width: 28px;
  height: 28px;
  place-items: center;
  border: 1px solid var(--accent);
  border-radius: 8px;
  background: var(--accent-subtle);
}

.settings-title h2 {
  margin: 0 0 4px;
  font-size: 14px;
  font-weight: 750;
}

.settings-title p {
  margin: 0;
  color: var(--muted);
  font-size: 11px;
}

.settings-form {
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

.form-field {
  display: flex;
  flex-direction: column;
  gap: 6px;
  color: #59645c;
  font-size: 10px;
  font-weight: 750;
}

.form-field.full-width {
  grid-column: 1 / -1;
}

.required {
  color: var(--danger);
  margin-left: 2px;
}

.field {
  padding: 9px 11px;
  border: 1px solid #dce2da;
  border-radius: 7px;
  background: #fff;
  color: var(--ink);
  font-size: 12px;
  transition: border-color 0.2s, box-shadow 0.2s;
}

.field:focus {
  outline: none;
  border-color: var(--accent);
  box-shadow: 0 0 0 3px var(--accent-focus);
}

.field::placeholder {
  color: #a0aaa1;
}

textarea.field {
  resize: vertical;
  min-height: 60px;
}

select.field {
  cursor: pointer;
}

small {
  color: var(--muted);
  font-size: 10px;
  margin-top: 4px;
}

.checkbox-field {
  flex-direction: row !important;
  align-items: center;
  gap: 10px;
  padding: 4px 0;
  font-weight: 600;
  color: var(--fg);
}

.checkbox-field input[type="checkbox"] {
  width: 18px;
  height: 18px;
  accent-color: var(--accent);
}

.form-actions {
  display: flex;
  align-items: center;
  gap: 16px;
  padding-top: 8px;
  border-top: 1px solid var(--line);
  margin-top: 8px;
}

.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  min-height: 39px;
  padding: 8px 13px;
  border: 1px solid transparent;
  border-radius: 7px;
  font-size: 11px;
  font-weight: 800;
  cursor: pointer;
  transition: filter 0.13s ease, transform 0.13s ease;
}

.btn-primary {
  background: var(--accent);
  color: var(--accent-ink);
}

.btn-primary:hover:not(:disabled) {
  filter: brightness(0.96);
  transform: translateY(-1px);
}

.btn-primary:disabled {
  opacity: 0.55;
  cursor: not-allowed;
}

.animate-spin {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

.settings-saved {
  color: var(--success);
  font-size: 11px;
  display: flex;
  align-items: center;
  gap: 6px;
  font-weight: 600;
}

.settings-options {
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.settings-option {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 16px;
  padding: 12px 14px;
  border: 1px solid var(--line);
  border-radius: 8px;
  background: var(--surface);
  cursor: pointer;
  transition: all 0.2s;
}

.settings-option:hover {
  border-color: var(--accent);
  background: var(--accent-subtle);
}

.settings-option span {
  flex: 1;
  min-width: 0;
}

.settings-option strong {
  display: block;
  font-size: 12px;
  color: var(--fg);
  font-weight: 600;
}

.settings-option small {
  display: block;
  margin-top: 2px;
  font-size: 10px;
  color: var(--muted);
  line-height: 1.4;
}

.settings-option input[type="checkbox"] {
  width: 20px;
  height: 20px;
  accent-color: var(--accent);
  flex-shrink: 0;
}

.alert-box {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 11px 13px;
  border: 1px solid #f0d2ca;
  border-radius: 7px;
  background: #fff7f4;
  color: #9c4938;
  font-size: 11px;
}

@media (max-width: 640px) {
  .form-row {
    grid-template-columns: 1fr;
  }
  
  .settings-grid {
    grid-template-columns: 1fr;
  }
  
  .form-actions {
    flex-direction: column;
    align-items: stretch;
  }
  
  .btn {
    width: 100%;
  }
}
</style>