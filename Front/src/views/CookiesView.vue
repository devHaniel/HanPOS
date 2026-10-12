<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { Cookie, Shield, Lock, Eye, Database, Truck, Globe, Mail, Phone, Calendar, Trash2, Download, UserCheck, AlertCircle, Settings, FileText } from '@lucide/vue'
import { useNegocioStore } from '@/Store'

const router = useRouter()
const negocioStore = useNegocioStore()

const negocio = computed(() => negocioStore.negocio)

const today = new Date().toLocaleDateString('es-HN', {
  year: 'numeric',
  month: 'long',
  day: 'numeric'
})

const sections = [
  {
    id: 'introduccion',
    title: '1. Introducción',
    icon: Shield,
    content: `
      <p>Esta <strong>Política de Privacidad y Cookies</strong> explica cómo <strong>HanPOS</strong> ("nosotros", "nuestro", "la Plataforma") recopila, usa, protege y comparte su información personal cuando utiliza nuestro sistema de punto de venta.</p>
      <p>Al usar HanPOS, usted acepta las prácticas descritas en esta política. Si no está de acuerdo, por favor no utilice el Servicio.</p>
    `
  },
  {
    id: 'responsable',
    title: '2. Responsable del Tratamiento',
    icon: Database,
    content: `
      <p><strong>HanPOS</strong> es el responsable del tratamiento de sus datos personales.</p>
      <ul>
        <li><strong>Denominación social:</strong> HanPOS S.A. de C.V.</li>
        <li><strong>Dirección:</strong> {{ negocio.value?.direccion || 'Tegucigalpa, Honduras' }}</li>
        <li><strong>Email DPO:</strong> <a href="mailto:privacidad@hanpos.com">privacidad@hanpos.com</a></li>
        <li><strong>Teléfono:</strong> {{ negocio.value?.telefono || '+504 2230-0000' }}</li>
      </ul>
    `
  },
  {
    id: 'datos-recopilados',
    title: '3. Datos que Recopilamos',
    icon: UserCheck,
    content: `
      <h3>3.1 Datos de Identificación y Contacto</h3>
      <ul>
        <li>Nombre, apellido, email, teléfono</li>
        <li>Nombre de usuario, contraseña (hasheada)</li>
        <li>Rol y permisos en la organización</li>
      </ul>
      <h3>3.2 Datos de Facturación y Fiscales</h3>
      <ul>
        <li>Nombre/Razón social del negocio</li>
        <li>RTN/NIT/Identificación fiscal</li>
        <li>Dirección fiscal, teléfono, email del negocio</li>
        <li>Configuración de moneda, zona horaria, mensaje pie de ticket</li>
      </ul>
      <h3>3.3 Datos Operativos (Generados por Uso)</h3>
      <ul>
        <li>Productos, categorías, precios, stock, códigos de barras</li>
        <li>Clientes: nombre, contacto, RTN, historial de compras</li>
        <li>Proveedores: nombre, contacto, condiciones comerciales</li>
        <li>Ventas: Items, totales, métodos de pago, fechas, cajero</li>
        <li>Compras: Items, totales, proveedor, fechas</li>
        <li>Caja: Aperturas, cierres, arqueos, movimientos, diferencias</li>
        <li>Usuarios: Accesos, acciones, auditoría</li>
      </ul>
      <h3>3.4 Datos Técnicos y de Navegación</h3>
      <ul>
        <li>Dirección IP, tipo de navegador, sistema operativo</li>
        <li>Páginas visitadas, tiempo de sesión, clicks</li>
        <li>Logs de errores, rendimiento, seguridad</li>
      </ul>
    `
  },
  {
    id: 'finalidad',
    title: '4. Finalidad y Base Legal del Tratamiento',
    icon: Lock,
    content: `
      <table class="purpose-table">
        <thead>
          <tr><th>Finalidad</th><th>Base Legal</th><th>Datos</th></tr>
        </thead>
        <tbody>
          <tr><td>Prestación del Servicio POS</td><td>Ejecución de contrato</td><td>Todos los operativos</td></tr>
          <tr><td>Facturación y cobro de suscripción</td><td>Ejecución de contrato / Obligación legal</td><td>Facturación, pago</td></tr>
          <tr><td>Cumplimiento obligaciones fiscales</td><td>Obligación legal</td><td>Ventas, compras, caja</td></tr>
          <tr><td>Soporte técnico y atención al cliente</td><td>Interés legítimo / Consentimiento</td><td>Contacto, logs</td></tr>
          <tr><td>Mejora del Servicio y analítica</td><td>Interés legítimo</td><td>Datos técnicos, uso</td></tr>
          <tr><td>Seguridad y prevención de fraude</td><td>Interés legítimo / Obligación legal</td><td>IP, logs, accesos</td></tr>
          <tr><td>Comunicaciones comerciales (opcional)</td><td>Consentimiento</td><td>Email, nombre</td></tr>
        </tbody>
      </table>
    `
  },
  {
    id: 'cookies',
    title: '5. Cookies y Tecnologías Similares',
    icon: Cookie,
    content: `
      <p>Utilizamos cookies y tecnologías similares (localStorage, sessionStorage) para operar el Servicio.</p>
      
      <h3>5.1 Cookies Estrictamente Necesarias (Siempre activas)</h3>
      <table class="cookie-table">
        <thead><tr><th>Nombre</th><th>Proveedor</th><th>Duración</th><th>Propósito</th></tr></thead>
        <tbody>
          <tr><td><code>auth_token</code></td><td>HanPOS</td><td>Sesión / 30 días</td><td>Autenticación JWT, mantener sesión</td></tr>
          <tr><td><code>refresh_token</code></td><td>HanPOS</td><td>30 días</td><td>Renovación silenciosa de token</td></tr>
          <tr><td><code>tenant_id</code></td><td>HanPOS</td><td>Sesión</td><td>Identificar organización activa</td></tr>
          <tr><td><code>csrf_token</code></td><td>HanPOS</td><td>Sesión</td><td>Protección CSRF en formularios</td></tr>
          <tr><td><code>cookie_consent</code></td><td>HanPOS</td><td>1 año</td><td>Recordar preferencias de cookies</td></tr>
        </tbody>
      </table>

      <h3>5.2 Cookies de Preferencias (Opcionales)</h3>
      <table class="cookie-table">
        <thead><tr><th>Nombre</th><th>Proveedor</th><th>Duración</th><th>Propósito</th></tr></thead>
        <tbody>
          <tr><td><code>theme</code></td><td>HanPOS</td><td>1 año</td><td>Tema claro/oscuro</td></tr>
          <tr><td><code>sidebar_state</code></td><td>HanPOS</td><td>Sesión</td><td>Sidebar colapsado/expandido</td></tr>
          <tr><td><code>pos_shortcuts</code></td><td>HanPOS</td><td>1 año</td><td>Atajos de teclado personalizados</td></tr>
        </tbody>
      </table>

      <h3>5.3 Cookies Analíticas (Opcionales - Requieren Consentimiento)</h3>
      <p>Usamos <strong>Google Analytics 4</strong> y <strong>Plausible</strong> para entender el uso de la plataforma.</p>
      <table class="cookie-table">
        <thead><tr><th>Nombre</th><th>Proveedor</th><th>Duración</th><th>Propósito</th></tr></thead>
        <tbody>
          <tr><td><code>_ga</code>, <code>_ga_*</code></td><td>Google</td><td>2 años</td><td>Distinguir usuarios, sesiones</td></tr>
          <tr><td><code>plausible_session</code></td><td>Plausible</td><td>30 min</td><td>Analytics privacidad-first</td></tr>
        </tbody>
      </table>

      <h3>5.4 Gestión de Cookies</h3>
      <ul>
        <li>Puede aceptar/rechazar cookies opcionales en el banner inferior.</li>
        <li>Puede cambiar preferencias en cualquier momento desde <RouterLink to="/settings" class="link">Configuración → Privacidad</RouterLink>.</li>
        <li>Puede borrar cookies desde su navegador (esto cerrará su sesión).</li>
        <li>Desactivar cookies necesarias romperá el funcionamiento del Sistema.</li>
      </ul>
    `
  },
  {
    id: 'compartir',
    title: '6. Compartir y Transferencia de Datos',
    icon: Globe,
    content: `
      <p><strong>No vendemos sus datos personales.</strong> Solo los compartimos en estos casos:</p>
      <ul>
        <li><strong>Proveedores de servicios (Subprocesadores):</strong> AWS (hosting), Stripe/PayPal (pagos), SendGrid (email), Twilio (SMS), Google/Plausible (analytics). Todos bajo contratos DPA/SCCs.</li>
        <li><strong>Obligación legal:</strong> Autoridades fiscales, judiciales o regulatorias competentes.</li>
        <li><strong>Protección de derechos:</strong> Para defender nuestros derechos, prevenir fraude o proteger seguridad.</li>
        <li><strong>Transacciones corporativas:</strong> En caso de fusión, adquisición o venta de activos (con notificación previa).</li>
      </ul>
      <p>No transferimos datos fuera de Honduras/EE.UU. sin garantías adecuadas (cláusulas tipo, decisiones de adecuación).</p>
    `
  },
  {
    id: 'seguridad',
    title: '7. Seguridad de la Información',
    icon: Shield,
    content: `
      <ul>
        <li><strong>Cifrado en tránsito:</strong> TLS 1.3 en todas las comunicaciones.</li>
        <li><strong>Cifrado en reposo:</strong> AES-256 en bases de datos y backups.</li>
        <li><strong>Autenticación:</strong> JWT con rotación de refresh tokens, expiración configurable.</li>
        <li><strong>Control de acceso:</strong> RBAC (roles), principio de menor privilegio.</li>
        <li><strong>Auditoría:</strong> Logs inmutables de accesos y acciones críticas.</li>
        <li><strong>Backups:</strong> Diarios, cifrados, replicados cross-region, test de restauración mensuales.</li>
        <li><strong>Certificaciones:</strong> Infraestructura en AWS (SOC 2, ISO 27001, PCI DSS Nivel 1).</li>
      </ul>
    `
  },
  {
    id: 'retencion',
    title: '8. Retención y Eliminación de Datos',
    icon: Calendar,
    content: `
      <table class="retention-table">
        <thead><tr><th>Categoría</th><th>Periodo</th><th>Base</th></tr></thead>
        <tbody>
          <tr><td>Datos de cuenta (usuario activo)</td><td>Mientras la cuenta esté activa</td><td>Contrato</td></tr>
          <tr><td>Datos de cuenta (eliminada)</td><td>30 días (recuperación) → anonimización</td><td>Interés legítimo</td></tr>
          <tr><td>Facturas, ventas, compras, caja</td><td>5 años mínimo</td><td>Código de Comercio / Ley Tributaria HN</td></tr>
          <tr><td>Logs de acceso y seguridad</td><td>12 meses</td><td>Interés legítimo / Seguridad</td></tr>
          <tr><td>Analytics anonimizados</td><td>26 meses</td><td>Consentimiento / Interés legítimo</td></tr>
          <tr><td>Backups</td><td>90 días (rolling)</td><td>Resiliencia operativa</td></tr>
        </tbody>
      </table>
      <p>Transcurridos los plazos, los datos se eliminan de forma segura (criptoborrado) o anonimizan irreversiblemente.</p>
    `
  },
  {
    id: 'derechos',
    title: '9. Sus Derechos (ARCO + GDPR)',
    icon: UserCheck,
    content: `
      <p>Usted tiene derecho a:</p>
      <ul>
        <li><strong>Acceso:</strong> Obtener confirmación y copia de sus datos.</li>
        <li><strong>Rectificación:</strong> Corregir datos inexactos o incompletos.</li>
        <li><strong>Supresión ("Olvido"):</strong> Eliminar datos cuando ya no sean necesarios.</li>
        <li><strong>Oposición:</strong> Oponerse al tratamiento por interés legítimo o marketing.</li>
        <li><strong>Limitación:</strong> Restringir el tratamiento temporalmente.</li>
        <li><strong>Portabilidad:</strong> Recibir sus datos en formato estructurado (JSON/CSV).</li>
        <li><strong>No decisiones automatizadas:</strong> No ser sujeto de decisiones solo automatizadas con efectos jurídicos.</li>
        <li><strong>Retirar consentimiento:</strong> En cualquier momento (sin afectar licitud previa).</li>
      </ul>
      <p><strong>Ejercicio:</strong> Envíe solicitud a <a href="mailto:privacidad@hanpos.com">privacidad@hanpos.com</a> con asunto "Derechos ARCO". Responderemos en <strong>15 días hábiles</strong> (prorrogables 15 más por complejidad).</p>
    `
  },
  {
    id: 'menores',
    title: '10. Menores de Edad',
    icon: AlertCircle,
    content: `
      <p>El Servicio no está dirigido a menores de 18 años. No recopilamos intencionadamente datos de menores. Si detectamos datos de menor sin consentimiento parental verificable, los eliminaremos de inmediato.</p>
    `
  },
  {
    id: 'cambios',
    title: '11. Cambios a esta Política',
    icon: FileText,
    content: `
      <p>Actualizaremos esta política cuando cambien nuestras prácticas o la normativa. Le notificaremos cambios materiales:</p>
      <ul>
        <li>Por email (si tenemos su consentimiento de comunicaciones).</li>
        <li>Mediante banner en el Sistema al iniciar sesión.</li>
        <li>Con al menos <strong>30 días de antelación</strong> a la entrada en vigor.</li>
      </ul>
      <p>El uso continuado tras notificación implica aceptación.</p>
    `
  },
  {
    id: 'contacto',
    title: '12. Contacto y Autoridad de Control',
    icon: Mail,
    content: `
      <p><strong>Delegado de Protección de Datos (DPO):</strong></p>
      <ul>
        <li>Email: <a href="mailto:privacidad@hanpos.com">privacidad@hanpos.com</a></li>
        <li>Teléfono: {{ negocio.value?.telefono || '+504 2230-0000' }}</li>
        <li>Dirección: {{ negocio.value?.direccion || 'Tegucigalpa, Honduras' }}</li>
      </ul>
      <p><strong>Autoridad de control:</strong> Si considera que vulneramos sus derechos, puede presentar reclamación ante la autoridad de protección de datos de su jurisdicción (en Honduras: CONADEH / Instituto de Acceso a la Información Pública).</p>
    `
  }
]

function scrollTo(id: string) {
  const el = document.getElementById(id)
  if (el) {
    el.scrollIntoView({ behavior: 'smooth', block: 'start' })
    history.pushState(null, '', `#${id}`)
  }
}
</script>

<template>
  <section class="fade-up legal-page">
    <div class="page-heading">
      <div>
        <p class="eyebrow">Legal</p>
        <h1>Política de Privacidad y Cookies</h1>
        <p class="text-muted">Última actualización: {{ today }}</p>
      </div>
    </div>

    <div class="legal-layout">
      <!-- Sidebar TOC -->
      <aside class="legal-toc" aria-label="Tabla de contenidos">
        <nav>
          <ul>
            <li v-for="section in sections" :key="section.id">
              <a :href="`#${section.id}`" @click.prevent="scrollTo(section.id)">
                {{ section.title }}
              </a>
            </li>
          </ul>
        </nav>
      </aside>

      <!-- Main Content -->
      <main class="legal-content" role="main">
        <section 
          v-for="section in sections" 
          :key="section.id" 
          :id="section.id" 
          class="legal-section"
        >
          <header class="section-header">
            <component :is="section.icon" class="section-icon" :size="20" stroke-width="2" />
            <h2>{{ section.title }}</h2>
          </header>
          <div class="section-body" v-html="section.content"></div>
        </section>

        <footer class="legal-footer">
          <p><strong>HanPOS</strong> — Sistema de Punto de Venta</p>
          <p class="text-muted">
            Consulte también nuestros <RouterLink to="/terminos" class="link">Términos y Condiciones</RouterLink>.
          </p>
        </footer>
      </main>
    </div>

    <!-- Cookie Banner (demo - real implementation would be in App.vue) -->
    <div class="cookie-banner-demo" v-show="false">
      <div class="cookie-banner-content">
        <component :is="Cookie" class="cookie-icon" :size="24" />
        <div class="cookie-text">
          <strong>Valoramos su privacidad</strong>
          <p>Usamos cookies necesarias para el funcionamiento y opcionales para analítica. <a href="/cookies#cookies">Más información</a>.</p>
        </div>
        <div class="cookie-actions">
          <button class="btn btn-secondary btn-sm">Solo necesarias</button>
          <button class="btn btn-primary btn-sm">Aceptar todas</button>
          <button class="btn btn-ghost btn-sm">Configurar</button>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
.legal-page {
  max-width: 1100px;
  margin: 0 auto;
}

.legal-layout {
  display: grid;
  grid-template-columns: 260px 1fr;
  gap: 48px;
  margin-top: 24px;
}

@media (max-width: 900px) {
  .legal-layout {
    grid-template-columns: 1fr;
    gap: 24px;
  }
  
  .legal-toc {
    position: sticky;
    top: 100px;
    max-height: calc(100vh - 120px);
    overflow-y: auto;
  }
}

.legal-toc nav ul {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.legal-toc a {
  display: block;
  padding: 8px 12px;
  color: var(--color-muted);
  font-size: 13px;
  font-weight: 500;
  border-radius: 6px;
  text-decoration: none;
  transition: all 0.15s;
  border-left: 3px solid transparent;
}

.legal-toc a:hover,
.legal-toc a.active {
  color: var(--color-accent);
  background: var(--color-accent-subtle);
  border-left-color: var(--color-accent);
}

.legal-content {
  display: flex;
  flex-direction: column;
  gap: 32px;
}

.legal-section {
  padding: 24px;
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: 12px;
}

.section-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 16px;
  padding-bottom: 12px;
  border-bottom: 1px solid var(--color-border);
}

.section-icon {
  color: var(--color-accent);
  flex-shrink: 0;
}

.section-header h2 {
  margin: 0;
  font-size: 16px;
  font-weight: 700;
  color: var(--color-fg);
}

.section-body {
  color: var(--color-fg);
  line-height: 1.7;
  font-size: 14px;
}

.section-body h3 {
  margin: 20px 0 12px;
  font-size: 15px;
  font-weight: 700;
  color: var(--color-fg);
}

.section-body h3:first-child {
  margin-top: 0;
}

.section-body p {
  margin: 0 0 12px;
}

.section-body p:last-child {
  margin-bottom: 0;
}

.section-body ul {
  margin: 12px 0 12px 20px;
  padding: 0;
}

.section-body li {
  margin-bottom: 8px;
  line-height: 1.6;
}

.section-body li:last-child {
  margin-bottom: 0;
}

.section-body strong {
  font-weight: 700;
}

.section-body code {
  background: var(--color-surface-hover);
  padding: 2px 6px;
  border-radius: 4px;
  font-size: 12px;
  font-family: monospace;
}

.section-body a.link {
  color: var(--color-accent);
  text-decoration: underline;
  text-underline-offset: 2px;
}

.section-body a.link:hover {
  color: var(--color-accent-hover);
}

/* Tables */
.purpose-table,
.cookie-table,
.retention-table {
  width: 100%;
  border-collapse: collapse;
  margin: 16px 0;
  font-size: 13px;
}

.purpose-table th,
.purpose-table td,
.cookie-table th,
.cookie-table td,
.retention-table th,
.retention-table td {
  padding: 10px 12px;
  border: 1px solid var(--color-border);
  text-align: left;
  vertical-align: top;
}

.purpose-table th,
.cookie-table th,
.retention-table th {
  background: var(--color-surface-hover);
  font-weight: 700;
  color: var(--color-fg);
}

.purpose-table tr:nth-child(even),
.cookie-table tr:nth-child(even),
.retention-table tr:nth-child(even) {
  background: var(--color-surface-hover);
}

.legal-footer {
  margin-top: 16px;
  padding: 20px 24px;
  background: var(--color-accent-subtle);
  border: 1px solid var(--color-accent);
  border-radius: 12px;
  text-align: center;
}

.legal-footer p {
  margin: 0 0 8px;
  color: var(--color-fg);
}

.legal-footer p:last-child {
  margin-bottom: 0;
}

/* Cookie banner demo */
.cookie-banner-demo {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  z-index: 1000;
  padding: 16px 24px;
  background: var(--color-surface);
  border-top: 1px solid var(--color-border);
  box-shadow: 0 -4px 20px rgba(0,0,0,0.1);
}

.cookie-banner-content {
  max-width: 1100px;
  margin: 0 auto;
  display: flex;
  align-items: flex-start;
  gap: 16px;
  flex-wrap: wrap;
}

.cookie-icon {
  color: var(--color-accent);
  flex-shrink: 0;
  margin-top: 2px;
}

.cookie-text {
  flex: 1;
  min-width: 280px;
}

.cookie-text strong {
  display: block;
  margin-bottom: 4px;
}

.cookie-text p {
  margin: 0;
  font-size: 13px;
  color: var(--color-muted);
}

.cookie-text a {
  color: var(--color-accent);
  text-decoration: underline;
}

.cookie-actions {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

@media (max-width: 640px) {
  .cookie-banner-content {
    flex-direction: column;
  }
  
  .cookie-actions {
    width: 100%;
  }
  
  .cookie-actions .btn {
    flex: 1;
  }
}
</style>