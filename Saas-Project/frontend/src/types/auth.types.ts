export interface UsuarioPerfil {
  id: string
  nombre: string
  email: string
  rol: 'Admin' | 'Agente' | 'Solicitante'
  tenantId: string
  tenantNombre: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  accessToken: string
  expiraEn: number
  usuario: UsuarioPerfil
}
