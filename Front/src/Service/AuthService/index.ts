import { apiClient } from '../ApiClient'
import type {
  AuthResponse,
  ChangePasswordRequest,
  LoginRequest,
  RegisterRequest,
  RevokeTokenResponse,
  User,
} from '@/Type/Auth'

function roleFromAccessToken(token: string): User['role'] {
  try {
    const payload = token.split('.')[1]
    if (!payload) return undefined
    const normalized = payload.replace(/-/g, '+').replace(/_/g, '/')
      .padEnd(Math.ceil(payload.length / 4) * 4, '=')
    const claims = JSON.parse(atob(normalized)) as Record<string, unknown>
    const claim = claims.role ?? claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
    const role = Array.isArray(claim) ? claim[0] : claim
    return role === 'Admin' || role === 'Vendedor' ? role : undefined
  } catch {
    return undefined
  }
}

function acceptAuth(response: AuthResponse): AuthResponse {
  apiClient.setTokens(response.token, response.refreshToken, response.expiration)
  response.usuario.role ??= roleFromAccessToken(response.token)
  return response
}

export const authService = {
  async login(request: LoginRequest): Promise<AuthResponse> {
    return acceptAuth(await apiClient.post<AuthResponse>('/auth/login', request, false))
  },

  async register(request: RegisterRequest): Promise<AuthResponse> {
    return acceptAuth(await apiClient.post<AuthResponse>('/auth/register', request, false))
  },

  async getCurrentUser(): Promise<User> {
    const user = await apiClient.get<User>('/auth/me')
    user.role ??= apiClient.accessTokenValue ? roleFromAccessToken(apiClient.accessTokenValue) : undefined
    return user
  },

  changePassword(request: ChangePasswordRequest): Promise<{ message: string }> {
    return apiClient.post('/auth/change-password', request)
  },

  revokeToken(refreshToken: string): Promise<RevokeTokenResponse> {
    return apiClient.post('/auth/revoke-token', { refreshToken })
  },

  logout(): void {
    apiClient.clearTokens()
  },
}