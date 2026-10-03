<script setup lang="ts">
import { ref } from 'vue'
import { Eye, EyeOff, LockKeyhole, Store } from '@lucide/vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/Store'
const route = useRoute()
const router = useRouter()
const auth = useAuthStore()
const username = ref('')
const password = ref('')
const showPassword = ref(false)
const recoveryNotice = ref(false)
async function submit() {
  try {
    await auth.login({ username: username.value.trim(), password: password.value })
    const target = route.query.redirect
    await router.replace(typeof target === 'string' && target.startsWith('/') && !target.startsWith('//') ? target : '/pos')
  } catch { /* The store supplies the visible error. */ }
}
</script>
<template>
  <main class="login-screen">
    <section class="login-art"><RouterLink to="/login" class="pos-brand"><span class="pos-brand-mark">M</span><span><strong>MercaPOS</strong><small>Gestión de ventas</small></span></RouterLink><div class="login-art-copy"><p class="eyebrow" style="color:#b9e773">Punto de venta</p><h1>Tu operación, en orden.</h1><p>Ventas, inventario y caja en un solo lugar, listos para el ritmo del mostrador.</p></div><div class="login-art-footer">Acceso seguro para el equipo de trabajo</div></section>
    <section class="login-form-side">
      <RouterLink to="/login" class="login-mobile-brand"><span class="pos-brand-mark">M</span><span><strong>MercaPOS</strong><small>Venta rápida</small></span></RouterLink>
      <form class="login-form" @submit.prevent="submit">
        <p class="eyebrow">Acceso al sistema</p><h2>Iniciar sesión</h2><p>Ingresa tus datos para continuar.</p>
        <label class="form-field" for="username">Usuario<div class="login-input-wrap"><Store :size="16"/><input id="username" v-model="username" autocomplete="username" required maxlength="50" placeholder="Tu usuario" /></div></label>
        <label class="form-field" for="password">Contraseña<div class="login-input-wrap"><LockKeyhole :size="16"/><input id="password" v-model="password" :type="showPassword ? 'text' : 'password'" autocomplete="current-password" required maxlength="100" placeholder="Tu contraseña"/><button type="button" class="password-toggle" :aria-label="showPassword ? 'Ocultar contraseña' : 'Mostrar contraseña'" @click="showPassword = !showPassword"><component :is="showPassword ? EyeOff : Eye" :size="16"/></button></div></label>
        <div class="login-recovery-row"><span></span><button type="button" @click="recoveryNotice = !recoveryNotice">¿Olvidaste tu contraseña?</button></div>
        <div v-if="recoveryNotice" class="login-recovery-note">Solicita el restablecimiento al administrador de tu tienda.</div>
        <div v-if="auth.error" class="alert-box">{{ auth.error }}</div><button class="btn login-submit" type="submit" :disabled="auth.isLoading">{{ auth.isLoading ? 'Validando…' : 'Entrar' }}</button>
      </form>
    </section>
  </main>
</template>
