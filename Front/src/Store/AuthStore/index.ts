import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { apiClient, authService } from '@/Service'
import type { AuthResponse, ChangePasswordRequest, LoginRequest, RegisterRequest, User } from '@/Type/Auth'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const isAuthenticated = computed(() => Boolean(user.value && apiClient.hasAccessToken))
  const isAdmin = computed(() => user.value?.role === 'Admin')

  async function runAuth<T>(operation: () => Promise<T>): Promise<T> {
    isLoading.value = true
    error.value = null
    try {
      return await operation()
    } catch (cause) {
      error.value = cause instanceof Error ? cause.message : 'No se pudo completar la operación'
      throw cause
    } finally {
      isLoading.value = false
    }
  }

  function acceptResponse(response: AuthResponse): AuthResponse {
    user.value = response.usuario
    return response
  }

  async function login(request: LoginRequest): Promise<AuthResponse> {
    return runAuth(async () => acceptResponse(await authService.login(request)))
  }

  async function register(request: RegisterRequest): Promise<AuthResponse> {
    return runAuth(async () => acceptResponse(await authService.register(request)))
  }

  async function initializeAuth(): Promise<void> {
    if (!apiClient.hasAccessToken) return
    await runAuth(async () => {
      user.value = await authService.getCurrentUser()
    })
  }

  async function changePassword(request: ChangePasswordRequest): Promise<void> {
    await runAuth(() => authService.changePassword(request))
  }

  async function logout(): Promise<void> {
    const refreshToken = apiClient.refreshToken
    try {
      if (refreshToken) await authService.revokeToken(refreshToken)
    } finally {
      authService.logout()
      user.value = null
      error.value = null
    }
  }

  function clearError(): void {
    error.value = null
  }

  return { user, isLoading, error, isAuthenticated, isAdmin, login, register, initializeAuth, changePassword, logout, clearError }
})