import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/Store'
import { useNegocioStore } from '@/Store'

// Simple in-flight request cache for negocio fetch
let negocioFetchPromise: Promise<void> | null = null

async function ensureNegocioLoaded(negocioStore: ReturnType<typeof useNegocioStore>): Promise<void> {
  if (negocioStore.negocio) return
  if (negocioFetchPromise) return negocioFetchPromise
  negocioFetchPromise = negocioStore.fetch().then(() => {}).finally(() => { negocioFetchPromise = null })
  return negocioFetchPromise
}

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: () => import('../views/PosLayout.vue'),
      redirect: '/pos',
      children: [
        { path: 'pos', name: 'pos', component: () => import('../views/PointOfSaleView.vue'), meta: { title: 'Punto de venta' } },
        { path: 'dashboard', name: 'dashboard', component: () => import('../views/HomeView.vue'), meta: { title: 'Resumen' } },
        { path: 'products', name: 'products', component: () => import('../views/DataManagerView.vue'), props: { resource: 'products' }, meta: { title: 'Productos' } },
        { path: 'categories', name: 'categories', component: () => import('../views/DataManagerView.vue'), props: { resource: 'categories' }, meta: { title: 'Categorías' } },
        { path: 'clients', name: 'clients', component: () => import('../views/DataManagerView.vue'), props: { resource: 'clients' }, meta: { title: 'Clientes' } },
        { path: 'providers', name: 'providers', component: () => import('../views/DataManagerView.vue'), props: { resource: 'providers' }, meta: { title: 'Proveedores' } },
        { path: 'users', name: 'users', component: () => import('../views/DataManagerView.vue'), props: { resource: 'users' }, meta: { title: 'Usuarios' } },
        { path: 'sales', name: 'sales', component: () => import('../views/SalesHistoryView.vue'), meta: { title: 'Ventas' } },
        { path: 'purchases', name: 'purchases', component: () => import('../views/PurchasesView.vue'), meta: { title: 'Compras / Abastecimiento' } },
        { path: 'purchases/history', name: 'purchases-history', component: () => import('../views/PurchasesHistoryView.vue'), meta: { title: 'Historial de Compras' } },
        { path: 'cash-register', name: 'cash-register', component: () => import('../views/CashRegisterView.vue'), meta: { title: 'Caja' } },
        { path: 'cash-history', name: 'cash-history', component: () => import('../views/CashRegisterHistoryView.vue'), meta: { title: 'Historial de Cajas' } },
        { path: 'movements', name: 'movements', component: () => import('../views/CashMovementsView.vue'), meta: { title: 'Movimientos' } },
        { path: 'settings', name: 'settings', component: () => import('../views/SettingsView.vue'), meta: { title: 'Configuración' } },
      ],
    },
    { path: '/login', name: 'login', component: () => import('../views/LoginView.vue'), meta: { public: true, title: 'Iniciar sesión' } },
    { path: '/terminos', name: 'terms', component: () => import('../views/TermsView.vue'), meta: { public: true, title: 'Términos y Condiciones' } },
    { path: '/cookies', name: 'cookies', component: () => import('../views/CookiesView.vue'), meta: { public: true, title: 'Política de Privacidad y Cookies' } },
    { path: '/:pathMatch(.*)*', redirect: '/' },
  ],
})

router.beforeEach(async (to) => {
  const authStore = useAuthStore()
  const negocioStore = useNegocioStore()
  
  // Public routes (login)
  if (to.meta.public) {
    if (authStore.isAuthenticated) {
      // Authenticated user hitting login
      if (to.name === 'login') {
        await ensureNegocioLoaded(negocioStore)
        
        const hasNegocio = !!negocioStore.negocio
        
        // If negocio exists → go to POS
        if (hasNegocio) {
          return { name: 'pos' }
        }
        // No negocio yet → allow login (user will be redirected to settings after login)
        return true
      }
      return { name: 'pos' }
    }
    // Not authenticated → allow login
    return true
  }
  
  // Protected routes - require auth
  if (!authStore.isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }
  
  // For protected routes (except settings), check if business is configured
  if (to.name !== 'settings') {
    await ensureNegocioLoaded(negocioStore)
    if (!negocioStore.negocio) {
      // No business configured → force settings
      return { name: 'settings', query: { redirect: to.fullPath, setup: 'required' } }
    }
  }
  
  return true
})

export default router