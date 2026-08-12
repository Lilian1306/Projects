<template>
  <header data-testid="app-nav" class="bg-slate-900 border-b border-slate-800 sticky top-0 z-30 shadow-md">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex items-center justify-between h-16">
        <!-- Logo / Marca -->
        <div class="flex items-center gap-3">
          <router-link to="/solicitudes" class="flex items-center gap-2.5 text-white font-bold text-lg tracking-tight hover:opacity-90 transition-opacity">
            <div class="w-9 h-9 bg-brand-600 border border-brand-500 rounded-lg flex items-center justify-center text-white shadow-md shadow-brand-600/30">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18.364 5.636l-3.536 3.536m0 5.656l3.536 3.536M9.172 9.172L5.636 5.636m3.536 9.192l-3.536 3.536M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-5 0a4 4 0 11-8 0 4 4 0 018 0z" />
              </svg>
            </div>
            <span>MesaSitec</span>
          </router-link>

          <!-- Separador & Tenant Badge -->
          <div v-if="authStore.tenantNombre" class="hidden md:flex items-center gap-2 pl-3 border-l border-slate-800">
            <span class="px-2.5 py-1 text-xs font-semibold bg-slate-800 text-slate-300 border border-slate-700/60 rounded-full flex items-center gap-1.5">
              <span class="w-1.5 h-1.5 rounded-full bg-emerald-400"></span>
              {{ authStore.tenantNombre }}
            </span>
          </div>
        </div>

        <!-- Usuario & Acciones -->
        <div v-if="authStore.isAuthenticated" class="flex items-center gap-3">
          <!-- Info de Usuario -->
          <div class="flex items-center gap-2.5 px-3 py-1.5 bg-slate-800/50 border border-slate-700/40 rounded-xl">
            <div class="w-7 h-7 rounded-lg bg-slate-700/60 border border-slate-600/40 flex items-center justify-center text-slate-300 font-bold text-xs uppercase shrink-0">
              {{ authStore.usuario?.nombre?.charAt(0) || 'U' }}
            </div>
            <div class="hidden sm:flex flex-col items-center">
              <div data-testid="nav-usuario-nombre" class="text-xs font-semibold text-white leading-tight text-center">
                {{ authStore.usuario?.nombre }}
              </div>
              <div class="mt-0.5 flex justify-center">
                <span 
                  data-testid="nav-usuario-rol"
                  class="px-1.5 py-0.5 text-[9px] font-bold rounded border uppercase tracking-wider inline-block leading-none text-center"
                  :class="getRolClass(authStore.userRole)"
                >
                  {{ authStore.userRole }}
                </span>
              </div>
            </div>
          </div>

          <!-- Botón Explícito Mi Perfil -->
          <button 
            data-testid="btn-mi-perfil"
            @click="showPerfilModal = true"
            title="Ver Mi Perfil"
            class="flex items-center gap-1.5 px-3 py-1.5 bg-brand-500/10 hover:bg-brand-500/20 text-brand-300 hover:text-white border border-brand-500/30 hover:border-brand-500/60 rounded-xl text-xs font-semibold transition-all cursor-pointer shadow-sm hover:shadow-brand-500/20"
          >
            <svg class="w-4 h-4 text-brand-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
            </svg>
            <span class="hidden md:inline">Mi Perfil</span>
          </button>

          <!-- Botón Logout -->
          <button 
            data-testid="btn-logout"
            @click="handleLogout"
            title="Cerrar Sesión"
            class="p-2 text-slate-400 hover:text-red-400 hover:bg-red-500/10 rounded-xl transition-all border border-transparent hover:border-red-500/20"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
            </svg>
          </button>
        </div>
      </div>
    </div>

    <!-- Modal Mi Perfil -->
    <div v-if="showPerfilModal" data-testid="modal-perfil" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-950/80 backdrop-blur-sm">
      <div class="bg-slate-900 border border-slate-800 rounded-2xl max-w-md w-full p-6 shadow-2xl space-y-5 relative">
        <!-- Header Modal -->
        <div class="flex items-center justify-between border-b border-slate-800 pb-4">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-xl bg-brand-600/20 border border-brand-500/30 flex items-center justify-center text-brand-400 font-bold text-base">
              {{ authStore.usuario?.nombre?.charAt(0) || 'U' }}
            </div>
            <div>
              <h3 class="font-bold text-white text-base">Mi Perfil de Usuario</h3>
              <p class="text-xs text-slate-400">Información de cuenta e identificador GUID</p>
            </div>
          </div>
          <button @click="showPerfilModal = false" class="text-slate-400 hover:text-white transition-colors p-1">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <!-- Detalles del Perfil -->
        <div class="space-y-3.5 text-xs">
          <div class="bg-slate-950/60 p-3 rounded-xl border border-slate-800/80 space-y-1">
            <span class="text-slate-400 text-[10px] font-semibold uppercase tracking-wider block">Nombre Completo</span>
            <div class="font-semibold text-white text-sm">{{ authStore.usuario?.nombre }}</div>
          </div>

          <div class="grid grid-cols-2 gap-3">
            <div class="bg-slate-950/60 p-3 rounded-xl border border-slate-800/80 space-y-1">
              <span class="text-slate-400 text-[10px] font-semibold uppercase tracking-wider block">Correo Electrónico</span>
              <div class="font-medium text-slate-200 truncate">{{ authStore.usuario?.email }}</div>
            </div>

            <div class="bg-slate-950/60 p-3 rounded-xl border border-slate-800/80 space-y-1">
              <span class="text-slate-400 text-[10px] font-semibold uppercase tracking-wider block">Rol en el Sistema</span>
              <div>
                <span class="px-2 py-0.5 text-[10px] font-bold rounded-lg border uppercase tracking-wider inline-block" :class="getRolClass(authStore.userRole)">
                  {{ authStore.userRole }}
                </span>
              </div>
            </div>
          </div>

          <div class="bg-slate-950/60 p-3 rounded-xl border border-slate-800/80 space-y-1">
            <span class="text-slate-400 text-[10px] font-semibold uppercase tracking-wider block">Organización / Empresa</span>
            <div class="font-semibold text-emerald-400 flex items-center gap-1.5">
              <span class="w-1.5 h-1.5 rounded-full bg-emerald-400"></span>
              {{ authStore.tenantNombre }}
            </div>
          </div>

          <!-- GUID de Usuario con Botón Copiar -->
          <div class="bg-brand-950/30 p-3.5 rounded-xl border border-brand-500/30 space-y-2">
            <div class="flex items-center justify-between">
              <span class="text-brand-400 text-[10px] font-bold uppercase tracking-wider">Identificador Único (GUID)</span>
              <span v-if="copiado" class="text-[10px] font-bold text-emerald-400 bg-emerald-500/10 px-2 py-0.5 rounded border border-emerald-500/20 animate-pulse">
                ¡Copiado al portapapeles!
              </span>
            </div>
            <div class="flex items-center gap-2">
              <code class="flex-1 p-2 bg-slate-950 rounded-lg text-slate-300 font-mono text-[11px] border border-slate-800 select-all truncate">
                {{ authStore.usuario?.id }}
              </code>
              <button 
                @click="copiarGuid" 
                class="px-3 py-2 bg-brand-600 hover:bg-brand-500 text-white font-semibold text-xs rounded-lg transition-colors shadow-md flex items-center gap-1 shrink-0"
              >
                <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                </svg>
                <span>Copiar</span>
              </button>
            </div>
          </div>
        </div>

        <!-- Footer Modal -->
        <div class="pt-2 flex justify-end border-t border-slate-800">
          <button @click="showPerfilModal = false" class="px-4 py-2 bg-slate-800 hover:bg-slate-700 text-slate-200 text-xs font-semibold rounded-xl transition-colors">
            Cerrar
          </button>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { toast } from 'vue3-toastify'
import { useAuthStore } from '@/stores/authStore'

const router = useRouter()
const authStore = useAuthStore()

const showPerfilModal = ref(false)
const copiado = ref(false)

function copiarGuid() {
  if (!authStore.usuario?.id) return
  navigator.clipboard.writeText(authStore.usuario.id)
  copiado.value = true
  toast.success('¡GUID copiado al portapapeles!')
  setTimeout(() => {
    copiado.value = false
  }, 2000)
}

const ROL_CLASSES: Record<string, string> = {
  Admin: 'bg-purple-500/10 text-purple-400 border-purple-500/30',
  Agente: 'bg-blue-500/10 text-blue-400 border-blue-500/30',
  Solicitante: 'bg-emerald-500/10 text-emerald-400 border-emerald-500/30',
}
function getRolClass(rol: string): string {
  return ROL_CLASSES[rol] || 'bg-emerald-500/10 text-emerald-400 border-emerald-500/30'
}

function handleLogout() {
  authStore.logout()
  toast.info('Sesión cerrada correctamente.')
  router.push('/login')
}
</script>

