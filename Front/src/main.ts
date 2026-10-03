import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'
import { useAuthStore } from './Store'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)

async function startApp() {
	const authStore = useAuthStore(pinia)
	await authStore.initializeAuth()

	app.use(router)
	await router.isReady()
	app.mount('#app')
}

void startApp()
