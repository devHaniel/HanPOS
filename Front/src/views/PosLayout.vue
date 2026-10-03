<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue'
import { RouterLink, RouterView, useRoute, useRouter } from 'vue-router'
import { ArrowLeftRight, Boxes, Building2, ClipboardList, LayoutGrid, LogOut, Menu, Settings, ShoppingCart, Tags, Users, Wallet, X } from '@lucide/vue'
import { useAuthStore } from '@/Store'
const route = useRoute()
const router = useRouter()
const auth = useAuthStore()
const mobileOpen = ref(false)
const navigation = [
  { label: 'Nueva venta', to: '/pos', icon: ShoppingCart },
  { label: 'Historial de ventas', to: '/sales', icon: ClipboardList },
  { label: 'Productos', to: '/products', icon: Boxes },
  { label: 'Categorías', to: '/categories', icon: Tags },
  { label: 'Clientes', to: '/clients', icon: Users },
  { label: 'Proveedores', to: '/providers', icon: Building2 },
  { label: 'Compras', to: '/purchases', icon: ClipboardList },
  { label: 'Caja', to: '/cash-register', icon: Wallet },
  { label: 'Movimientos', to: '/movements', icon: ArrowLeftRight },
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
      <nav class="pos-nav" aria-label="Navegación principal"><RouterLink v-for="item in navigation" :key="item.to" :to="item.to" class="pos-nav-link" :class="{ active: isActive(item.to) }" @click="mobileOpen = false"><component :is="item.icon" class="pos-nav-icon" :size="18" :stroke-width="1.8"/><span>{{ item.label }}</span></RouterLink></nav>
      <RouterLink class="pos-library-link" to="/component-library" @click="mobileOpen = false"><LayoutGrid :size="16"/> Biblioteca de componentes</RouterLink>
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
