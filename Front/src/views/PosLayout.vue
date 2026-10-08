<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue'
import { RouterLink, RouterView, useRoute, useRouter } from 'vue-router'
import { ArrowLeftRight, Boxes, Building2, ClipboardList, LayoutGrid, LogOut, Menu, Settings, ShoppingCart, Tags, Users, Wallet, X, Archive, ChevronDown, ChevronUp } from '@lucide/vue'
import { useAuthStore } from '@/Store'
const route = useRoute()
const router = useRouter()
const auth = useAuthStore()
const mobileOpen = ref(false)
const moreOpen = ref(false)

// Main navigation - only the essentials
const mainNavigation = [
  { label: 'Nueva venta', to: '/pos', icon: ShoppingCart },
  { label: 'Resumen del día', to: '/dashboard', icon: LayoutGrid },
  { label: 'Inventario', to: '/products', icon: Boxes },
  { label: 'Compras', to: '/purchases', icon: ClipboardList },
  { label: 'Caja', to: '/cash-register', icon: Wallet },
]

// Advanced features under "Más"
const moreNavigation = [
  { label: 'Historial de ventas', to: '/sales', icon: ClipboardList },
  { label: 'Historial de cajas', to: '/cash-history', icon: Archive },
  { label: 'Movimientos de caja', to: '/movements', icon: ArrowLeftRight },
  { label: 'Clientes', to: '/clients', icon: Users },
  { label: 'Proveedores', to: '/providers', icon: Building2 },
  { label: 'Categorías', to: '/categories', icon: Tags },
  { label: 'Configuración', to: '/settings', icon: Settings },
]

function isActive(path: string) { return route.path === path || (path !== '/pos' && route.path.startsWith(`${path}/`)) }
async function logout() { try { await auth.logout() } finally { await router.replace('/login') } }
function onAuthExpired() { auth.user = null; router.replace({ name: 'login', query: { redirect: route.fullPath } }) }
onMounted(() => window.addEventListener('auth:expired', onAuthExpired))
onUnmounted(() => window.removeEventListener('auth:expired', onAuthExpired))
</script>
<template>
  <div class="pos-layout">
    <button class="sidebar-scrim" :class="{ visible: mobileOpen }" aria-label="Cerrar navegación" @click="mobileOpen = false" />
    <aside class="pos-sidebar" :class="{ open: mobileOpen }">
      <RouterLink to="/pos" class="pos-brand" @click="mobileOpen = false"><span class="pos-brand-mark">M</span><span><strong>HanPOS</strong><small>Venta rápida</small></span></RouterLink>
      <p class="pos-nav-label">Menú principal</p>
      <nav class="pos-nav" aria-label="Navegación principal">
        <RouterLink v-for="item in mainNavigation" :key="item.to" :to="item.to" class="pos-nav-link" :class="{ active: isActive(item.to) }" @click="mobileOpen = false">
          <component :is="item.icon" class="pos-nav-icon" :size="18" :stroke-width="1.8"/>
          <span>{{ item.label }}</span>
        </RouterLink>
        
        <!-- Más dropdown -->
        <div class="pos-nav-more">
          <button 
            class="pos-nav-link more-trigger" 
            @click="moreOpen = !moreOpen"
            :class="{ active: moreOpen }"
            :aria-expanded="moreOpen ? 'true' : 'false'"
            aria-label="Más opciones"
          >
            <component :is="moreOpen ? ChevronUp : ChevronDown" class="pos-nav-icon" :size="18" :stroke-width="1.8"/>
            <span>Más</span>
          </button>
          
          <Transition name="slide">
            <div v-show="moreOpen" class="pos-nav-more-content">
              <RouterLink v-for="item in moreNavigation" :key="item.to" :to="item.to" class="pos-nav-link more-item" :class="{ active: isActive(item.to) }" @click="mobileOpen = false; moreOpen = false">
                <component :is="item.icon" class="pos-nav-icon" :size="16" :stroke-width="1.8"/>
                <span>{{ item.label }}</span>
              </RouterLink>
            </div>
          </Transition>
        </div>
      </nav>
      <div class="pos-sidebar-footer"><span class="user-avatar">{{ (auth.user?.nombre || auth.user?.username || 'U').slice(0,1).toUpperCase() }}</span><span><strong>{{ auth.user?.nombre || auth.user?.username || 'Usuario' }}</strong><small>Sesión activa</small></span></div>
    </aside>
    <div class="pos-main">
      <header class="pos-topbar"><div class="topbar-left"><button class="mobile-menu-button" aria-label="Abrir navegación" @click="mobileOpen = !mobileOpen"><component :is="mobileOpen ? X : Menu" :size="19"/></button><div><span class="topbar-kicker">HanPOS</span><h2 class="topbar-title">{{ route.meta.title || 'Ventas' }}</h2></div></div>
        <div class="topbar-actions"><span class="topbar-status"><i></i> Sistema listo</span><button class="btn btn-secondary topbar-logout" @click="logout"><LogOut :size="15"/><span>Salir</span></button></div>
      </header>
      <main class="pos-workspace"><RouterView /></main>
    </div>
  </div>
</template>
