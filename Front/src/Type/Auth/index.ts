export type UserRole = 'Admin' | 'Vendedor'

export interface User {
  id: number
  nombre: string
  username: string
  passwordHash: string
  email?: string
  activo: boolean
  role?: UserRole
}

export interface LoginRequest {
  username: string
  password: string
}

export interface RegisterRequest {
  nombre: string
  username: string
  email: string
  password: string
  confirmPassword: string
}

export interface AuthResponse {
  token: string
  refreshToken: string
  expiration: string
  usuario: User
}

export interface RefreshTokenRequest {
  refreshToken: string
}

export interface ChangePasswordRequest {
  currentPassword: string
  newPassword: string
  confirmNewPassword: string
}

export interface RevokeTokenResponse {
  message: string
}