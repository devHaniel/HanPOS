<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { ArrowRight, Banknote, Check, CreditCard, Minus, Package, Plus, Search, ShoppingCart, Tag, Trash2, UserRound, X, Printer } from '@lucide/vue'
import { cashRegisterService, categoryService, clientService, saleService } from '@/Service'
import { useAuthStore, useProductStore } from '@/Store'
import type { CashRegister } from '@/Type/CashRegister'
import type { Category } from '@/Type/Category'
import type { Client } from '@/Type/Client'
import type { Product } from '@/Type/Product'

interface CartEntry { product: Product; quantity: number }
type CheckoutStep = 'summary' | 'payment' | 'success' | null

const auth = useAuthStore()
const productStore = useProductStore()
const products = ref<Product[]>([])
const categories = ref<Category[]>([])
const clients = ref<Client[]>([])
const register = ref<CashRegister | null>(null)
const cart = ref<CartEntry[]>([])
const search = ref('')
const selectedCategoryId = ref<number | null>(null)
const selectedClientId = ref<number | null>(null)
const paymentMethod = ref<1 | 2 | 3>(1)
const cashReceived = ref(0)
const checkoutStep = ref<CheckoutStep>(null)
const lastSale = ref<{
  id: number
  total: number
  change: number
  items: CartEntry[]
  paymentMethod: 1 | 2 | 3
  clientName: string
  date: string
  subtotal: number
  tax: number
} | null>(null)
const busy = ref(false)
const error = ref('')
const notice = ref('')

const filteredProducts = computed(() => products.value.filter((product) => {
  const matchesSearch = `${product.nombre} ${product.codigo}`.toLowerCase().includes(search.value.toLowerCase().trim())
  const matchesCategory = selectedCategoryId.value === null || product.categoriaId === selectedCategoryId.value
  return product.activo && product.stock > 0 && matchesSearch && matchesCategory
}))
const itemCount = computed(() => cart.value.reduce((sum, entry) => sum + entry.quantity, 0))
const subtotal = computed(() => cart.value.reduce((sum, entry) => sum + entry.product.precioVenta * entry.quantity, 0))
// Los precios ya incluyen IVA 15%. El impuesto es la parte del subtotal que corresponde al IVA.
const tax = computed(() => Math.round(subtotal.value * 0.15 / 1.15 * 100) / 100)
const discount = computed(() => 0)
const total = computed(() => subtotal.value)
const change = computed(() => Math.max(0, cashReceived.value - total.value))
const currency = (amount: number) => new Intl.NumberFormat('es-HN', { style: 'currency', currency: 'HNL' }).format(amount)

function categoryProductCount(categoryId: number) {
  return products.value.filter((product) => product.categoriaId === categoryId).length
}

function addToCart(product: Product) {
  const line = cart.value.find((entry) => entry.product.id === product.id)
  if (line) {
    if (line.quantity < product.stock) line.quantity++
  } else {
    cart.value.push({ product, quantity: 1 })
  }
  error.value = ''
  notice.value = ''
}

function changeQuantity(line: CartEntry, amount: number) {
  const quantity = line.quantity + amount
  if (quantity <= 0) cart.value = cart.value.filter((entry) => entry !== line)
  else if (quantity <= line.product.stock) line.quantity = quantity
}

function beginCheckout() {
  if (!register.value) {
    error.value = 'Abre una caja para comenzar a vender.'
    return
  }
  if (!cart.value.length) return
  checkoutStep.value = 'summary'
  error.value = ''
}

async function completeSale() {
  if (!register.value || !auth.user?.id || !cart.value.length) return
  if (paymentMethod.value === 1 && cashReceived.value < total.value) {
    error.value = 'El monto recibido debe cubrir el total.'
    return
  }
  busy.value = true
  error.value = ''
  try {
    const completed = await saleService.create({
      usuarioId: auth.user.id,
      cajaId: register.value.id,
      clienteId: selectedClientId.value,
      metodoPago: paymentMethod.value,
      detalles: cart.value.map(({ product, quantity }) => ({ productoId: product.id, cantidad: quantity })),
    })
    
    const client = clients.value.find(c => c.id === selectedClientId.value)
    
    lastSale.value = {
      id: completed.id,
      total: completed.total,
      change: paymentMethod.value === 1 ? Math.max(0, cashReceived.value - completed.total) : 0,
      items: [...cart.value],
      paymentMethod: paymentMethod.value,
      clientName: client?.nombre || 'Cliente ocasional',
      date: new Date().toLocaleString('es-HN', { dateStyle: 'short', timeStyle: 'short' }),
      subtotal: subtotal.value,
      tax: tax.value
    }
    
    checkoutStep.value = 'success'
    await load()
  } catch (cause) {
    error.value = cause instanceof Error ? cause.message : 'No se pudo completar la venta.'
  } finally {
    busy.value = false
  }
}

function newSale() {
  cart.value = []
  selectedClientId.value = null
  selectedCategoryId.value = null
  search.value = ''
  cashReceived.value = 0
  paymentMethod.value = 1
  checkoutStep.value = null
  lastSale.value = null
}

function generateReceipt(): string {
  if (!lastSale.value) return ''
  
  const W = 42 // thermal width in characters
  const sale = lastSale.value
  const nl = '\n'
  const hr = '='.repeat(W)
  const dr = '-'.repeat(W)
  const center = (text: string) => text.padStart((W + text.length) / 2).padEnd(W)
  const right = (text: string) => text.padStart(W)
  const left = (text: string) => text.padEnd(W)
  const lr = (l: string, r: string) => l.padEnd(W - r.length) + r
  
  const paymentLabel = sale.paymentMethod === 1 ? 'EFECTIVO' : sale.paymentMethod === 2 ? 'TARJETA' : 'TRANSFERENCIA'
  
  let out = ''
  
  // Header
  out += center('HANPOS') + nl
  out += center('Sistema de Ventas') + nl
  out += hr + nl
  out += center('TICKET DE VENTA') + nl
  out += hr + nl
  
  // Info
  out += left(`Venta #${sale.id.toString().padStart(6, '0')}`) + nl
  out += left(`Fecha: ${sale.date}`) + nl
  out += left(`Cajero: ${auth.user?.nombre || 'Usuario'}`) + nl
  out += left(`Cliente: ${sale.clientName}`) + nl
  out += dr + nl
  
  // Items header
  out += left('DESCRIPCION        CANT  PRECIO  IMPORTE') + nl
  out += dr + nl
  
  // Items
  for (const line of sale.items) {
    const name = line.product.nombre.substring(0, 18).padEnd(18)
    const qty = line.quantity.toString().padStart(4)
    const price = currency(line.product.precioVenta).padStart(7)
    const importe = currency(line.product.precioVenta * line.quantity).padStart(8)
    out += `${name}${qty}${price}${importe}` + nl
  }
  
  out += dr + nl
  
  // Totals
  out += lr('SUBTOTAL:', currency(sale.subtotal)) + nl
  out += lr('DESCUENTO:', currency(0)) + nl
  out += lr('IMPUESTO (15%):', currency(sale.tax)) + nl
  out += hr + nl
  out += lr('TOTAL:', currency(sale.total)) + nl
  out += hr + nl
  
  // Payment
  out += left(`PAGO: ${paymentLabel}`) + nl
  if (sale.paymentMethod === 1) {
    out += lr('RECIBIDO:', currency(sale.total + sale.change)) + nl
    out += lr('CAMBIO:', currency(sale.change)) + nl
  }
  out += hr + nl
  
  // Footer
  out += center('¡GRACIAS POR SU COMPRA!') + nl
  out += center('Vuelva pronto') + nl
  out += hr + nl
  out += center('www.hanpos.com') + nl
  
  return out
}

function printReceipt() {
  if (!lastSale.value) return
  
  const receipt = generateReceipt()
  const printWindow = window.open('', '_blank', 'width=400,height=600')
  if (!printWindow) return
  
  const html = `
    <!DOCTYPE html>
    <html>
    <head>
      <meta charset="utf-8">
      <title>Ticket Venta #${lastSale.value.id}</title>
      <style>
        @media print {
          @page { margin: 0; size: 80mm auto; }
          body { margin: 0; padding: 8px; }
          .no-print { display: none; }
        }
        body { 
          font-family: 'Courier New', 'Consolas', 'Monospace', monospace; 
          font-size: 11px; 
          line-height: 1.3;
          white-space: pre-wrap;
          padding: 8px;
          max-width: 320px;
          margin: 0 auto;
        }
        .print-btn { 
          display: block; 
          margin: 16px auto 0; 
          padding: 12px 24px; 
          background: #2563eb; 
          color: white; 
          border: none; 
          border-radius: 6px; 
          font-size: 14px; 
          cursor: pointer;
        }
        .print-btn:hover { background: #1d4ed8; }
      </style>
    </head>
    <body onload="window.print()">
      ${receipt}
      <button class="print-btn no-print" onclick="window.print()">Imprimir / Guardar PDF</button>
      <scr` + `ipt>setTimeout(() => window.close(), 10000)</scr` + `ipt>
    </body>
    </html>
  `
  printWindow.document.write(html)
  printWindow.document.close()
}

async function load() {
  const results = await Promise.allSettled([
    productStore.fetchAll({ pagina: 1, cantidad: 100 }),
    categoryService.list(),
    clientService.list(),
    cashRegisterService.getOpen(),
  ])
  if (results[0].status === 'fulfilled') products.value = productStore.items
  else error.value = 'No se pudo cargar el catálogo. Revisa tu conexión.'
  if (results[1].status === 'fulfilled') categories.value = results[1].value
  if (results[2].status === 'fulfilled') clients.value = results[2].value
  if (results[3].status === 'fulfilled') register.value = results[3].value
  else register.value = null
}

onMounted(load)
</script>

<template>
  <section class="sale-screen">
    <header class="sale-heading">
      <div class="sale-title-wrap">
        <div class="sale-title-icon"><ShoppingCart :size="21" :stroke-width="1.8" /></div>
        <div><p class="sale-overline">Mostrador / Venta rápida</p><h1>Nueva venta</h1></div>
      </div>
      <div class="sale-terminal-status"><span class="status-indicator" :class="{ offline: !register }"></span><span>{{ register ? `Caja #${register.id} abierta` : 'Caja cerrada' }}</span></div>
    </header>

    <div v-if="error" class="alert-box sale-alert" role="alert">{{ error }}<button aria-label="Cerrar aviso" @click="error = ''"><X :size="15" /></button></div>
    <div v-if="notice" class="sale-notice"><Check :size="16" />{{ notice }}</div>

    <div class="sales-workspace">
      <section class="catalog-area" aria-label="Catálogo de productos">
        <label class="sale-search-wrap"><Search :size="19" /><input v-model="search" autofocus placeholder="Buscar producto por nombre o código" aria-label="Buscar producto" /><kbd>⌘ K</kbd></label>
        <div class="category-filter" role="group" aria-label="Filtrar por categoría">
          <button class="category-filter-chip" :class="{ selected: selectedCategoryId === null }" @click="selectedCategoryId = null">Todos</button>
          <button v-for="category in categories" :key="category.id" class="category-filter-chip" :class="{ selected: selectedCategoryId === category.id }" @click="selectedCategoryId = category.id">
            {{ category.nombre }}<span>{{ categoryProductCount(category.id) }}</span>
          </button>
        </div>

        <div class="catalog-heading"><div><h2>Productos</h2><span>{{ filteredProducts.length }} disponibles</span></div><button class="catalog-view-button" title="Vista de cuadrícula" aria-label="Vista de cuadrícula"><Package :size="16" /></button></div>
        <div v-if="productStore.isLoading" class="catalog-state">Cargando productos…</div>
        <div v-else-if="!filteredProducts.length" class="catalog-empty"><div class="empty-icon"><Package :size="24" /></div><strong>No encontramos productos</strong><span>Prueba otra búsqueda o categoría.</span></div>
        <div v-else class="sale-product-grid">
          <button v-for="product in filteredProducts" :key="product.id" class="sale-product-card" :disabled="!register" @click="addToCart(product)">
            <span class="sale-product-visual"><Package :size="25" :stroke-width="1.55" /><span class="product-stock">Stock {{ product.stock }}</span></span>
            <span class="sale-product-name">{{ product.nombre }}</span>
            <span class="sale-product-meta">{{ product.codigo }}</span>
            <span class="sale-product-bottom"><strong>{{ currency(product.precioVenta) }}</strong><span class="add-product-button" aria-label="Agregar producto"><Plus :size="16" /></span></span>
          </button>
        </div>
      </section>

      <aside class="sale-cart" aria-label="Carrito de venta">
        <header class="sale-cart-header"><div><div class="sale-cart-title"><ShoppingCart :size="18"/><h2>Venta actual</h2></div><span class="sale-cart-subtitle">{{ itemCount }} {{ itemCount === 1 ? 'producto' : 'productos' }}</span></div><button class="cart-clear-button" :disabled="!cart.length" @click="cart = []"><Trash2 :size="15"/><span>Vaciar</span></button></header>
        <div class="sale-cart-client"><UserRound :size="16"/><select v-model="selectedClientId" aria-label="Cliente"><option :value="null">Cliente ocasional</option><option v-for="client in clients" :key="client.id" :value="client.id">{{ client.nombre }}</option></select><span class="client-caret">⌄</span></div>

        <div v-if="!cart.length" class="cart-empty"><div class="cart-empty-icon"><ShoppingCart :size="25" :stroke-width="1.6" /></div><strong>Agrega productos</strong><span>Selecciona productos del catálogo para iniciar la venta.</span></div>
        <div v-else class="sale-cart-items">
          <article v-for="line in cart" :key="line.product.id" class="sale-cart-item">
            <div class="cart-item-thumb"><Package :size="18" /></div>
            <div class="cart-item-info"><strong>{{ line.product.nombre }}</strong><span>{{ currency(line.product.precioVenta) }} c/u</span><div class="quantity-stepper"><button aria-label="Reducir cantidad" @click="changeQuantity(line, -1)"><Minus :size="13" /></button><span>{{ line.quantity }}</span><button aria-label="Aumentar cantidad" :disabled="line.quantity >= line.product.stock" @click="changeQuantity(line, 1)"><Plus :size="13" /></button></div></div>
            <div class="cart-item-price"><strong>{{ currency(line.product.precioVenta * line.quantity) }}</strong><button aria-label="Eliminar producto" @click="cart = cart.filter((entry) => entry !== line)"><X :size="15" /></button></div>
          </article>
        </div>

        <div class="sale-cart-footer">
          <div class="sale-total-row"><span>Subtotal</span><strong>{{ currency(subtotal) }}</strong></div>
          <div class="sale-total-row"><span>Descuento</span><strong>{{ currency(discount) }}</strong></div>
          <div class="sale-total-row"><span>Impuesto (15%)</span><strong>{{ currency(tax) }}</strong></div>
          <div class="sale-grand-total"><span>Total</span><strong>{{ currency(total) }}</strong></div>
          <button class="proceed-button" :disabled="!cart.length || !register" @click="beginCheckout">Proceder al pago <ArrowRight :size="17"/></button>
          <span class="cart-hint" v-if="!register">Abre una caja para poder cobrar.</span>
        </div>
      </aside>
    </div>

    <div v-if="checkoutStep === 'summary'" class="dialog-backdrop" @click.self="checkoutStep = null">
      <section class="checkout-dialog" role="dialog" aria-modal="true" aria-labelledby="summary-title">
        <header class="checkout-dialog-header"><div><span class="dialog-step-label">PASO 1 DE 2</span><h2 id="summary-title">Resumen de venta</h2><p>Revisa los productos antes de continuar.</p></div><button class="dialog-close" aria-label="Cerrar" @click="checkoutStep = null"><X :size="18"/></button></header>
        <div class="summary-items"><div v-for="line in cart" :key="line.product.id" class="summary-line"><div><strong>{{ line.product.nombre }}</strong><span>{{ line.quantity }} × {{ currency(line.product.precioVenta) }}</span></div><b>{{ currency(line.product.precioVenta * line.quantity) }}</b></div></div>
        <div class="summary-totals"><div><span>Subtotal</span><b>{{ currency(subtotal) }}</b></div><div><span>Descuento</span><b>{{ currency(discount) }}</b></div><div><span>Impuesto (15%)</span><b>{{ currency(tax) }}</b></div><div class="summary-total"><span>Total</span><b>{{ currency(total) }}</b></div></div>
        <footer class="checkout-dialog-actions"><button class="btn btn-secondary" @click="checkoutStep = null">Volver a la venta</button><button class="btn" @click="checkoutStep = 'payment'">Continuar al pago <ArrowRight :size="16"/></button></footer>
      </section>
    </div>

    <div v-if="checkoutStep === 'payment'" class="dialog-backdrop" @click.self="checkoutStep = 'summary'">
      <section class="checkout-dialog payment-dialog" role="dialog" aria-modal="true" aria-labelledby="payment-title">
        <header class="checkout-dialog-header"><div><span class="dialog-step-label">PASO 2 DE 2</span><h2 id="payment-title">Forma de pago</h2><p>Selecciona cómo paga el cliente.</p></div><button class="dialog-close" aria-label="Volver al resumen" @click="checkoutStep = 'summary'"><X :size="18"/></button></header>
        <div class="payment-total-card"><span>Total a cobrar</span><strong>{{ currency(total) }}</strong></div>
        <div class="payment-methods" role="radiogroup" aria-label="Método de pago">
          <button class="payment-method" :class="{ active: paymentMethod === 1 }" role="radio" :aria-checked="paymentMethod === 1" @click="paymentMethod = 1"><Banknote :size="20"/><span>Efectivo</span><i></i></button>
          <button class="payment-method" :class="{ active: paymentMethod === 2 }" role="radio" :aria-checked="paymentMethod === 2" @click="paymentMethod = 2"><CreditCard :size="20"/><span>Tarjeta</span><i></i></button>
          <button class="payment-method" :class="{ active: paymentMethod === 3 }" role="radio" :aria-checked="paymentMethod === 3" @click="paymentMethod = 3"><ArrowLeftRight :size="20"/><span>Transferencia</span><i></i></button>
        </div>
        <div v-if="paymentMethod === 1" class="cash-payment-fields"><label class="form-field" for="cash-received">Monto recibido<input id="cash-received" v-model.number="cashReceived" class="field cash-received-input" type="number" min="0" step="0.01" inputmode="decimal" placeholder="0.00" autofocus/></label><div class="cash-breakdown"><div><span>Total</span><b>{{ currency(total) }}</b></div><div><span>Recibido</span><b>{{ currency(cashReceived) }}</b></div><div class="cash-change"><span>Cambio</span><b>{{ currency(change) }}</b></div></div></div>
        <div v-if="error" class="alert-box payment-error" role="alert">{{ error }}</div>
        <footer class="checkout-dialog-actions"><button class="btn btn-secondary" @click="checkoutStep = 'summary'">Volver</button><button class="btn btn-success" :disabled="busy || (paymentMethod === 1 && cashReceived < total)" @click="completeSale">{{ busy ? 'Procesando…' : 'Confirmar venta' }} <Check :size="16"/></button></footer>
      </section>
    </div>

    <div v-if="checkoutStep === 'success'" class="dialog-backdrop success-backdrop">
      <section class="checkout-dialog success-dialog" role="dialog" aria-modal="true" aria-labelledby="success-title">
        <div class="success-mark"><Check :size="29" :stroke-width="2.3"/></div><p class="dialog-step-label">TRANSACCIÓN REGISTRADA</p><h2 id="success-title">Venta completada</h2><p class="success-copy">El cobro se registró correctamente.</p>
        <div class="success-total"><span>Total</span><strong>{{ currency(lastSale?.total ?? 0) }}</strong></div><div v-if="paymentMethod === 1" class="success-change"><span>Cambio</span><strong>{{ currency(lastSale?.change ?? 0) }}</strong></div>
        <div class="success-actions">
          <button class="btn btn-secondary" @click="printReceipt"><Printer :size="16"/> Imprimir ticket</button>
          <button class="btn btn-success" @click="newSale">Nueva venta</button>
        </div>
      </section>
    </div>
  </section>
</template>
+