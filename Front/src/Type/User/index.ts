export interface ManagedUser {
  id: number
  nombre: string
  userName: string
  email: string
  activo: boolean
  fechaCreacion: string
}

interface UserFields {
  nombre: string
  username: string
  email: string
  activo: boolean
}

export interface UserCreateRequest extends UserFields {
  password: string
}

export interface UserUpdateRequest extends UserFields {
  id: number
  password?: string
}