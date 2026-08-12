import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'
import { httpClient } from '@/api/httpClient'
import type { LoginRequest, LoginResponse, UsuarioPerfil } from '@/types'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const usuarioRaw = localStorage.getItem('usuario')
  const usuario = ref<UsuarioPerfil | null>(usuarioRaw ? JSON.parse(usuarioRaw) : null)
  const cargando = ref(false)
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => !!token.value)
  const userRole = computed(() => usuario.value?.rol || '')
  const tenantNombre = computed(() => usuario.value?.tenantNombre || '')

  async function login(credentials: LoginRequest): Promise<boolean> {
    cargando.value = true
    error.value = null
    try {
      const response = await httpClient.post<LoginResponse>('/auth/login', credentials)
      const data = response.data

      token.value = data.accessToken
      usuario.value = data.usuario

      localStorage.setItem('token', data.accessToken)
      localStorage.setItem('usuario', JSON.stringify(data.usuario))

      return true
    } catch (err: unknown) {
      error.value = (axios.isAxiosError<{ detail?: string }>(err) && err.response?.data?.detail)
        || 'Error al iniciar sesión. Verifique sus credenciales.'
      return false
    } finally {
      cargando.value = false
    }
  }

  async function fetchPerfil(): Promise<void> {
    if (!token.value) return
    try {
      const response = await httpClient.get<UsuarioPerfil>('/me')
      usuario.value = response.data
      localStorage.setItem('usuario', JSON.stringify(response.data))
    } catch (err) {
      console.error('Error al actualizar perfil de usuario', err)
    }
  }

  function logout(): void {
    token.value = null
    usuario.value = null
    error.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('usuario')
  }

  return {
    token,
    usuario,
    cargando,
    error,
    isAuthenticated,
    userRole,
    tenantNombre,
    login,
    fetchPerfil,
    logout
  }
})
