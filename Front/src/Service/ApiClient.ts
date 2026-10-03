import type { AuthResponse } from '@/Type/Auth'
import type { PageQuery, ProblemDetails } from '@/Type/Common'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:8080/api'
const ACCESS_TOKEN_KEY = 'hanpos.accessToken'
const REFRESH_TOKEN_KEY = 'hanpos.refreshToken'
const ACCESS_TOKEN_EXPIRATION_KEY = 'hanpos.accessTokenExpiration'

export class ApiError extends Error {
  constructor(
    message: string,
    public readonly status: number,
    public readonly details?: ProblemDetails,
    public readonly retryAfterSeconds?: number,
  ) {
    super(message)
    this.name = 'ApiError'
  }
}

type QueryParams = PageQuery | Record<string, string | number | boolean | null | undefined>

export class ApiClient {
  private accessToken: string | null = null
  private refreshTokenValue: string | null = null
  private accessTokenExpiresAt: number | null = null
  private refreshInFlight: Promise<void> | null = null

  constructor() {
    if (typeof window === 'undefined') return
    try {
      this.accessToken = window.sessionStorage.getItem(ACCESS_TOKEN_KEY)
      this.refreshTokenValue = window.sessionStorage.getItem(REFRESH_TOKEN_KEY)
      const expiration = Number(window.sessionStorage.getItem(ACCESS_TOKEN_EXPIRATION_KEY))
      this.accessTokenExpiresAt = Number.isFinite(expiration) && expiration > 0 ? expiration : null
    } catch {
      // Authentication remains in memory if browser storage is unavailable.
    }
  }

  get hasAccessToken(): boolean {
    return this.accessToken !== null
  }

  get accessTokenValue(): string | null {
    return this.accessToken
  }

  get refreshToken(): string | null {
    return this.refreshTokenValue
  }

  setTokens(accessToken: string, refreshToken: string, expiration?: string): void {
    this.accessToken = accessToken
    this.refreshTokenValue = refreshToken
    const parsedExpiration = expiration ? Date.parse(expiration) : Number.NaN
    this.accessTokenExpiresAt = Number.isFinite(parsedExpiration) ? parsedExpiration : null
    if (typeof window !== 'undefined') {
      try {
        window.sessionStorage.setItem(ACCESS_TOKEN_KEY, accessToken)
        window.sessionStorage.setItem(REFRESH_TOKEN_KEY, refreshToken)
        if (this.accessTokenExpiresAt !== null) {
          window.sessionStorage.setItem(ACCESS_TOKEN_EXPIRATION_KEY, String(this.accessTokenExpiresAt))
        } else {
          window.sessionStorage.removeItem(ACCESS_TOKEN_EXPIRATION_KEY)
        }
      } catch {
        // Keep the in-memory session when browser storage is unavailable.
      }
    }
  }

  clearTokens(): void {
    this.accessToken = null
    this.refreshTokenValue = null
    this.accessTokenExpiresAt = null
    if (typeof window !== 'undefined') {
      try {
        window.sessionStorage.removeItem(ACCESS_TOKEN_KEY)
        window.sessionStorage.removeItem(REFRESH_TOKEN_KEY)
        window.sessionStorage.removeItem(ACCESS_TOKEN_EXPIRATION_KEY)
      } catch {
        // In-memory state is still cleared when browser storage is unavailable.
      }
    }
  }

  async get<T>(path: string, query?: QueryParams): Promise<T> {
    const params = new URLSearchParams()
    for (const [key, value] of Object.entries(query ?? {})) {
      if (value !== undefined && value !== null) params.set(key, String(value))
    }
    const queryString = params.size > 0 ? `?${params.toString()}` : ''
    return this.request<T>(`${path}${queryString}`, { method: 'GET' })
  }

  post<T>(path: string, body?: unknown, requiresAuth = true): Promise<T> {
    return this.request<T>(path, { method: 'POST', body: body === undefined ? undefined : JSON.stringify(body) }, requiresAuth)
  }

  put<T>(path: string, body: unknown): Promise<T> {
    return this.request<T>(path, { method: 'PUT', body: JSON.stringify(body) })
  }

  delete<T = void>(path: string): Promise<T> {
    return this.request<T>(path, { method: 'DELETE' })
  }

  private async request<T>(path: string, init: RequestInit, requiresAuth = true, canRefresh = true): Promise<T> {
    if (requiresAuth && canRefresh && this.refreshTokenValue && this.shouldRefreshAccessToken()) {
      try {
        await this.refreshTokens()
      } catch (error) {
        this.expireSession()
        throw error
      }
    }

    const headers = new Headers(init.headers)
    headers.set('Accept', 'application/json')
    if (init.body !== undefined) headers.set('Content-Type', 'application/json')
    if (requiresAuth && this.accessToken) headers.set('Authorization', `Bearer ${this.accessToken}`)

    let response: Response
    try {
      response = await fetch(`${API_BASE_URL}${path}`, { ...init, headers })
    } catch (error) {
      throw new ApiError(error instanceof Error ? error.message : 'No se pudo conectar con la API', 0)
    }

    if (response.status === 401 && requiresAuth) {
      if (canRefresh && this.refreshTokenValue) {
        try {
          await this.refreshTokens()
        } catch (error) {
          this.expireSession()
          throw error
        }
        return this.request<T>(path, init, requiresAuth, false)
      }

      this.expireSession()
    }

    if (!response.ok) {
      const payload = await response.json().catch(() => null) as unknown
      const body = payload && typeof payload === 'object' ? payload as Record<string, unknown> : null
      const details = body && typeof body.status === 'number' ? body as unknown as ProblemDetails : undefined
      const message = (typeof payload === 'string' && payload)
        || (typeof body?.detail === 'string' && body.detail)
        || (typeof body?.message === 'string' && body.message)
        || (typeof body?.title === 'string' && body.title)
        || response.statusText
        || 'Error de API'
      const retryAfter = Number(response.headers.get('Retry-After'))
      throw new ApiError(message, response.status, details, Number.isFinite(retryAfter) ? retryAfter : undefined)
    }

    if (response.status === 204) return undefined as T
    return response.json() as Promise<T>
  }

  private async refreshTokens(): Promise<void> {
    if (!this.refreshInFlight) {
      this.refreshInFlight = (async () => {
        if (!this.refreshTokenValue) throw new ApiError('La sesión expiró', 401)
        const response = await fetch(`${API_BASE_URL}/auth/refresh-token`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json', Accept: 'application/json' },
          body: JSON.stringify({ refreshToken: this.refreshTokenValue }),
        })
        if (!response.ok) throw new ApiError('La sesión expiró', response.status)
        const auth = await response.json() as AuthResponse
        this.setTokens(auth.token, auth.refreshToken, auth.expiration)
      })().finally(() => {
        this.refreshInFlight = null
      })
    }
    return this.refreshInFlight
  }

  private shouldRefreshAccessToken(): boolean {
    return !this.accessToken || (this.accessTokenExpiresAt !== null && this.accessTokenExpiresAt <= Date.now() + 30_000)
  }

  private expireSession(): void {
    this.clearTokens()
    if (typeof window !== 'undefined') window.dispatchEvent(new CustomEvent('auth:expired'))
  }
}

export const apiClient = new ApiClient()