import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/Store'

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
        { path: 'purchases', name: 'purchases', component: () => import('../views/PurchasesView.vue'), meta: { title: 'Compras' } },
        { path: 'cash-register', name: 'cash-register', component: () => import('../views/CashRegisterView.vue'), meta: { title: 'Caja' } },
        { path: 'movements', name: 'movements', component: () => import('../views/CashMovementsView.vue'), meta: { title: 'Movimientos' } },
        { path: 'settings', name: 'settings', component: () => import('../views/SettingsView.vue'), meta: { title: 'Configuración' } },
        { path: 'component-library', name: 'component-library', component: () => import('../views/ComponentLibraryView.vue'), meta: { title: 'Biblioteca de componentes' } },
      ],
    },
    { path: '/login', name: 'login', component: () => import('../views/LoginView.vue'), meta: { public: true, title: 'Iniciar sesión' } },
    { path: '/:pathMatch(.*)*', redirect: '/' },
  ],
})

router.beforeEach((to) => {
  const authStore = useAuthStore()
  if (to.meta.public) return authStore.isAuthenticated ? { name: 'pos' } : true
  return authStore.isAuthenticated ? true : { name: 'login', query: { redirect: to.fullPath } }
})

export default router
