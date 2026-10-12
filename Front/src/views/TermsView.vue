<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ArrowLeft, FileText, Shield, Lock, UserCheck, Database, Clock, AlertCircle, Mail, Phone, Truck, CreditCard, Globe, Scale, Gavel } from '@lucide/vue'
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
    id: 'aceptacion',
    title: '1. Aceptación de los Términos',
    icon: FileText,
    content: `
      <p>Al acceder y utilizar <strong>HanPOS</strong> (en adelante, "el Sistema", "la Plataforma" o "el Servicio"), usted acepta quedar vinculado por estos Términos y Condiciones (en adelante, "los Términos"), así como por nuestra Política de Privacidad y Política de Cookies.</p>
      <p>Si no está de acuerdo con alguna parte de estos Términos, no debe utilizar el Sistema.</p>
    `
  },
  {
    id: 'descripcion',
    title: '2. Descripción del Servicio',
    icon: Shield,
    content: `
      <p>HanPOS es un sistema de punto de venta (POS) basado en la nube diseñado para la gestión de ventas, inventario, clientes, proveedores, compras, caja y reportes para negocios minoristas y de servicios.</p>
      <p>El Servicio se proporciona "tal cual" y "según disponibilidad", sin garantías de ningún tipo, ya sean expresas o implícitas.</p>
    `
  },
  {
    id: 'cuentas',
    title: '3. Cuentas de Usuario y Acceso',
    icon: UserCheck,
    content: `
      <ul>
        <li><strong>Registro:</strong> Para usar el Sistema, debe crear una cuenta proporcionando información veraz, actual y completa.</li>
        <li><strong>Credenciales:</strong> Es responsable de mantener la confidencialidad de su contraseña y de todas las actividades bajo su cuenta.</li>
        <li><strong>Notificación:</strong> Debe notificarnos de inmediato cualquier uso no autorizado de su cuenta.</li>
        <li><strong>Roles:</strong> El Sistema soporta roles (Administrador, Cajero, Gerente) con permisos diferenciados.</li>
      </ul>
    `
  },
  {
    id: 'datos',
    title: '4. Datos e Información del Negocio',
    icon: Database,
    content: `
      <ul>
        <li><strong>Titularidad:</strong> Usted conserva todos los derechos de propiedad intelectual sobre los datos que ingrese al Sistema (productos, clientes, proveedores, ventas, etc.).</li>
        <li><strong>Licencia de uso:</strong> Nos otorga una licencia no exclusiva, mundial y libre de regalías para almacenar, procesar y mostrar sus datos únicamente para prestar el Servicio.</li>
        <li><strong>Respaldos:</strong> Realizamos respaldos automáticos periódicos, pero no garantimos la imposibilidad de pérdida de datos. Se recomienda exportar reportes críticos regularmente.</li>
        <li><strong>Exportación:</strong> Puede solicitar la exportación completa de sus datos en formatos estándar (CSV, JSON, PDF) en cualquier momento.</li>
      </ul>
    `
  },
  {
    id: 'suscripcion',
    title: '5. Suscripción, Facturación y Pagos',
    icon: CreditCard,
    content: `
      <ul>
        <li><strong>Planes:</strong> El Servicio se ofrece bajo modelo SaaS con planes mensuales/anuales. Las características de cada plan se detallan en la página de precios.</li>
        <li><strong>Facturación:</strong> Los pagos se procesan a través de pasarelas de pago seguras (Stripe, PayPal, transferencia bancaria). No almacenamos números completos de tarjetas.</li>
        <li><strong>Renovación:</strong> Las suscripciones se renuevan automáticamente salvo cancelación previa.</li>
        <li><strong>Impuestos:</strong> Los precios pueden no incluir impuestos aplicables (IVA, ISV, etc.) según su jurisdicción.</li>
        <li><strong>Reembolsos:</strong> No se realizan reembolsos por periodos parciales no utilizados, salvo que la ley local lo exija.</li>
      </ul>
    `
  },
  {
    id: 'uso-aceptable',
    title: '6. Uso Aceptable',
    icon: Gavel,
    content: `
      <p>Usted se compromete a <strong>NO</strong> utilizar el Sistema para:</p>
      <ul>
        <li>Actividades ilegales, fraudulentas o que violen leyes aplicables (lavado de dinero, evasión fiscal, etc.).</li>
        <li>Transmitir virus, malware, ransomware o código malicioso.</li>
        <li>Intentar acceder sin autorización a sistemas, cuentas o datos de terceros.</li>
        <li>Interferir con la integridad, rendimiento o seguridad del Servicio.</li>
        <li>Revender, sublicenciar o distribuir el Servicio sin autorización escrita.</li>
        <li>Almacenar o procesar datos sensibles prohibidos (salud protegida, menores, etc.) sin cumplimiento normativo.</li>
      </ul>
      <p>Nos reservamos el derecho de suspender o terminar su acceso ante incumplimiento.</p>
    `
  },
  {
    id: 'propiedad-intelectual',
    title: '7. Propiedad Intelectual',
    icon: Lock,
    content: `
      <p>Todos los derechos de propiedad intelectual sobre el Sistema (código, diseño, marca, logos, documentación, algoritmos) son de <strong>HanPOS</strong> o sus licenciantes.</p>
      <p>Estos Términos no le otorgan derechos sobre nuestras marcas, nombres comerciales o logotipos.</p>
    `
  },
  {
    id: 'privacidad',
    title: '8. Privacidad y Protección de Datos',
    icon: Shield,
    content: `
      <p>El tratamiento de datos personales se rige por nuestra <a href="/cookies" class="link">Política de Privacidad y Cookies</a>. En resumen:</p>
      <ul>
        <li>Recopilamos datos de identificación, contacto, facturación y uso del Sistema.</li>
        <li>Los datos se usan para prestar el Servicio, facturar, mejorar la plataforma y cumplir obligaciones legales.</li>
        <li>No vendemos sus datos a terceros.</li>
        <li>Puede ejercer derechos ARCO (Acceso, Rectificación, Cancelación, Oposición) contactando a <a href="mailto:privacidad@hanpos.com">privacidad@hanpos.com</a>.</li>
      </ul>
    `
  },
  {
    id: 'disponibilidad',
    title: '9. Disponibilidad y Mantenimiento',
    icon: Clock,
    content: `
      <ul>
        <li>Nos esforzamos por mantener el Servicio disponible 99.9% del tiempo (excluyendo mantenimientos programados).</li>
        <li>Los mantenimientos programados se notificarán con 24h de antelación cuando sea posible.</li>
        <li>No somos responsables por interrupciones por causas de fuerza mayor, fallos de proveedores de internet, energía o terceros.</li>
      </ul>
    `
  },
  {
    id: 'limitacion-responsabilidad',
    title: '10. Limitación de Responsabilidad',
    icon: AlertCircle,
    content: `
      <p>EN LA MÁXIMA MEDIDA PERMITIDA POR LA LEY:</p>
      <ul>
        <li>HanPOS NO SERÁ RESPONSABLE por daños indirectos, incidentales, consecuentes, punitivos o por pérdida de beneficios, datos, uso o buena voluntad.</li>
        <li>Nuestra responsabilidad total acumulada no excederá el monto pagado por usted en los 12 meses previos al reclamo.</li>
        <li>El Sistema no sustituye asesoría contable, legal, fiscal o financiera profesional.</li>
      </ul>
    `
  },
  {
    id: 'indemnizacion',
    title: '11. Indemnización',
    icon: Scale,
    content: `
      <p>Usted acepta indemnizar, defender y mantener indemne a HanPOS, sus directivos, empleados y afiliados frente a cualquier reclamo, daño, pérdida, responsabilidad o gasto (incluidos honorarios legales razonables) que surja de:</p>
      <ul>
        <li>Su incumplimiento de estos Términos.</li>
        <li>Su uso del Sistema en violación de leyes o derechos de terceros.</li>
        <li>Contenido que usted ingrese, transmita o procese través del Sistema.</li>
      </ul>
    `
  },
  {
    id: 'terminacion',
    title: '12. Terminación',
    icon: FileText,
    content: `
      <ul>
        <li><strong>Por usted:</strong> Puede cancelar su suscripción en cualquier momento desde Configuración → Suscripción. El acceso continuará hasta el fin del periodo pagado.</li>
        <li><strong>Por nosotros:</strong> Podemos suspender o terminar su acceso por incumplimiento material, no pago, o si cesamos operaciones (con 30 días de aviso).</li>
        <li><strong>Efectos:</strong> Tras la terminación, tiene 30 días para exportar sus datos. Luego, procederemos a su eliminación segura según nuestra política de retención.</li>
      </ul>
    `
  },
  {
    id: 'modificaciones',
    title: '13. Modificaciones a los Términos',
    icon: FileText,
    content: `
      <p>Podemos actualizar estos Términos periódicamente. Le notificaremos cambios materiales por email o mediante aviso en el Sistema con al menos <strong>30 días de antelación</strong>. El uso continuado del Servicio tras la notificación constituye aceptación de los nuevos Términos.</p>
    `
  },
  {
    id: 'ley-aplicable',
    title: '14. Ley Aplicable y Jurisdicción',
    icon: Gavel,
    content: `
      <p>Estos Términos se rigen por las leyes de la <strong>República de Honduras</strong>. Cualquier disputa se someterá a la jurisdicción exclusiva de los tribunales competentes de <strong>Tegucigalpa, M.D.C.</strong>, renunciando a cualquier otro fuero.</p>
      <p>Para usuarios internacionales, esto no afecta derechos de protección al consumidor irrenunciables en su país de residencia.</p>
    `
  },
  {
    id: 'contacto',
    title: '15. Contacto',
    icon: Mail,
    content: `
      <p>Para consultas sobre estos Términos:</p>
      <ul>
        <li><strong>Email:</strong> <a href="mailto:legal@hanpos.com">legal@hanpos.com</a></li>
        <li><strong>Soporte:</strong> <a href="mailto:soporte@hanpos.com">soporte@hanpos.com</a></li>
        <li><strong>Dirección:</strong> {{ negocio.value?.direccion || 'Tegucigalpa, Honduras' }}</li>
        <li><strong>Teléfono:</strong> {{ negocio.value?.telefono || '+504 2230-0000' }}</li>
      </ul>
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
        <h1>Términos y Condiciones</h1>
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
            Estos términos son vinculantes a partir de su aceptación. 
            Consulte también nuestra <RouterLink to="/cookies">Política de Privacidad y Cookies</RouterLink>.
          </p>
        </footer>
      </main>
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

.section-body a.link {
  color: var(--color-accent);
  text-decoration: underline;
  text-underline-offset: 2px;
}

.section-body a.link:hover {
  color: var(--color-accent-hover);
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
</style>