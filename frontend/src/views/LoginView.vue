<template>
  <div class="min-h-screen bg-slate-900 flex items-center justify-center p-4 relative overflow-hidden">
    <!-- Background Decorator Gradients -->
    <div class="absolute -top-40 -left-40 w-96 h-96 bg-brand-600/30 rounded-full blur-3xl pointer-events-none"></div>
    <div class="absolute -bottom-40 -right-40 w-96 h-96 bg-purple-600/20 rounded-full blur-3xl pointer-events-none"></div>

    <div class="w-full max-w-md bg-slate-800/80 backdrop-blur-xl border border-slate-700/60 rounded-2xl p-8 shadow-2xl relative z-10">
      <!-- Header / Logo -->
      <div class="text-center mb-8">
        <div class="inline-flex items-center justify-center w-14 h-14 bg-brand-600/20 border border-brand-500/30 text-brand-400 rounded-xl mb-3 shadow-inner">
          <svg class="w-7 h-7" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18.364 5.636l-3.536 3.536m0 5.656l3.536 3.536M9.172 9.172L5.636 5.636m3.536 9.192l-3.536 3.536M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-5 0a4 4 0 11-8 0 4 4 0 018 0z" />
          </svg>
        </div>
        <h1 class="text-2xl font-bold text-white tracking-tight">MesaSitec</h1>
        <p class="text-slate-400 text-sm mt-1">Mesa de Ayuda Multi-Tenant</p>
      </div>

      <!-- Error Alert (Auth backend o validación frontend) -->
      <div v-if="mensajeError" data-testid="login-error" class="mb-6 p-4 bg-red-500/10 border border-red-500/30 rounded-xl flex items-start gap-3 text-red-400 text-sm">
        <svg class="w-5 h-5 shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <span>{{ mensajeError }}</span>
      </div>

      <!-- Formulario de Login -->
      <form @submit.prevent="handleLogin" class="space-y-5" novalidate>
        <div>
          <label class="block text-xs font-semibold text-slate-300 uppercase tracking-wider mb-2">Correo Electrónico</label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-slate-400">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 12a4 4 0 10-8 0 4 4 0 008 0zm0 0v1.5a2.5 2.5 0 005 0V12a9 9 0 10-9 9m4.5-1.206a8.959 8.959 0 01-4.5 1.207" />
              </svg>
            </div>
            <input 
              data-testid="login-email"
              v-model="email"
              type="email" 
              placeholder="correo@empresa.com"
              :class="[
                'w-full pl-10 pr-4 py-3 bg-slate-900/80 border rounded-xl text-white placeholder-slate-500 focus:outline-none transition-all text-sm',
                validationError && !email.trim() ? 'border-red-500 focus:ring-1 focus:ring-red-500' : 'border-slate-700 focus:border-brand-500 focus:ring-1 focus:ring-brand-500'
              ]"
            />
          </div>
        </div>

        <div>
          <label class="block text-xs font-semibold text-slate-300 uppercase tracking-wider mb-2">Contraseña</label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-slate-400">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
              </svg>
            </div>
            <input 
              data-testid="login-password"
              v-model="password"
              :type="showPassword ? 'text' : 'password'" 
              placeholder="••••••••"
              :class="[
                'w-full pl-10 pr-10 py-3 bg-slate-900/80 border rounded-xl text-white placeholder-slate-500 focus:outline-none transition-all text-sm',
                validationError && !password.trim() ? 'border-red-500 focus:ring-1 focus:ring-red-500' : 'border-slate-700 focus:border-brand-500 focus:ring-1 focus:ring-brand-500'
              ]"
            />
            <button 
              type="button" 
              @click="showPassword = !showPassword"
              class="absolute inset-y-0 right-0 pr-3 flex items-center text-slate-400 hover:text-slate-200 transition-colors"
            >
              <svg v-if="!showPassword" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
              </svg>
              <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858-5.908a10.03 10.03 0 013.682-.743c4.478 0 8.268 2.943 9.542 7a10.025 10.025 0 01-4.132 5.411m-6.102-6.102a3 3 0 11-4.243-4.243m4.243 4.243L3 3l18 18" />
              </svg>
            </button>
          </div>
        </div>

        <button 
          data-testid="login-submit"
          type="submit" 
          :disabled="authStore.cargando"
          class="w-full py-3 px-4 bg-brand-600 hover:bg-brand-500 text-white font-semibold rounded-xl shadow-lg shadow-brand-600/30 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:ring-offset-2 focus:ring-offset-slate-900 disabled:opacity-50 disabled:cursor-not-allowed transition-all flex items-center justify-center gap-2"
        >
          <svg v-if="authStore.cargando" class="animate-spin h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          <span>{{ authStore.cargando ? 'Iniciando sesión...' : 'Iniciar Sesión' }}</span>
        </button>
      </form>

      <!-- Selector Rápido de Cuentas de Prueba (Evaluadores - 5 Perfiles) -->
      <div class="mt-8 pt-6 border-t border-slate-700/60 space-y-4">
        <div class="flex items-center justify-between">
         <!-- <p class="text-xs font-semibold text-slate-300 uppercase tracking-wider">Acceso Rápido de Prueba</p> --> 
          <span class="text-[10px] text-slate-400 bg-slate-700/40 px-2 py-0.5 rounded-full border border-slate-600/40">5 Perfiles</span>
        </div>

        <!-- Cooperativa Norte (3 perfiles) -->
        <div class="space-y-1.5">
          <div class="text-[10px] font-bold text-slate-400 uppercase tracking-wider flex items-center gap-1.5">
            <span class="w-1.5 h-1.5 rounded-full bg-brand-400"></span>
            Cooperativa Norte
          </div>
          <div class="grid grid-cols-3 gap-2 text-xs">
            <button 
              @click="setQuickUser('admin@norte.test')"
              class="py-2 px-2 bg-slate-700/40 hover:bg-slate-700 text-slate-300 rounded-lg border border-slate-600/40 hover:border-purple-500/50 transition-all text-left group"
            >
              <span class="text-[10px] font-bold text-purple-400 uppercase block">Admin</span>
              <span class="text-[11px] text-white truncate block group-hover:text-purple-300">admin</span>
            </button>

            <button 
              @click="setQuickUser('agente1@norte.test')"
              class="py-2 px-2 bg-slate-700/40 hover:bg-slate-700 text-slate-300 rounded-lg border border-slate-600/40 hover:border-blue-500/50 transition-all text-left group"
            >
              <span class="text-[10px] font-bold text-blue-400 uppercase block">Agente</span>
              <span class="text-[11px] text-white truncate block group-hover:text-blue-300">agente1</span>
            </button>

            <button 
              @click="setQuickUser('user1@norte.test')"
              class="py-2 px-2 bg-slate-700/40 hover:bg-slate-700 text-slate-300 rounded-lg border border-slate-600/40 hover:border-emerald-500/50 transition-all text-left group"
            >
              <span class="text-[10px] font-bold text-emerald-400 uppercase block">Solicitante</span>
              <span class="text-[11px] text-white truncate block group-hover:text-emerald-300">user1</span>
            </button>
          </div>
        </div>

        <!-- Bufete Sur (3 perfiles) -->
        <div class="space-y-1.5 pt-1">
          <div class="text-[10px] font-bold text-slate-400 uppercase tracking-wider flex items-center gap-1.5">
            <span class="w-1.5 h-1.5 rounded-full bg-indigo-400"></span>
            Bufete Sur
          </div>
          <div class="grid grid-cols-3 gap-2 text-xs">
            <button 
              @click="setQuickUser('admin@sur.test')"
              class="py-2 px-2 bg-slate-700/40 hover:bg-slate-700 text-slate-300 rounded-lg border border-slate-600/40 hover:border-purple-500/50 transition-all text-left group"
            >
              <span class="text-[10px] font-bold text-purple-400 uppercase block">Admin</span>
              <span class="text-[11px] text-white truncate block group-hover:text-purple-300">admin</span>
            </button>

            <button 
              @click="setQuickUser('agente1@sur.test')"
              class="py-2 px-2 bg-slate-700/40 hover:bg-slate-700 text-slate-300 rounded-lg border border-slate-600/40 hover:border-blue-500/50 transition-all text-left group"
            >
              <span class="text-[10px] font-bold text-blue-400 uppercase block">Agente</span>
              <span class="text-[11px] text-white truncate block group-hover:text-blue-300">agente1</span>
            </button>

            <button 
              @click="setQuickUser('user1@sur.test')"
              class="py-2 px-2 bg-slate-700/40 hover:bg-slate-700 text-slate-300 rounded-lg border border-slate-600/40 hover:border-emerald-500/50 transition-all text-left group"
            >
              <span class="text-[10px] font-bold text-emerald-400 uppercase block">Solicitante</span>
              <span class="text-[11px] text-white truncate block group-hover:text-emerald-300">user1</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { toast } from 'vue3-toastify'
import { useAuthStore } from '@/stores/authStore'

const router = useRouter()
const authStore = useAuthStore()

const email = ref('')
const password = ref('')
const showPassword = ref(false)
const validationError = ref<string | null>(null)

const mensajeError = computed(() => {
  return validationError.value || authStore.error
})

function setQuickUser(selectedEmail: string) {
  email.value = selectedEmail
  password.value = 'Sitec.2026'
  validationError.value = null
  authStore.error = null
}

async function handleLogin() {
  validationError.value = null

  if (!email.value.trim() || !password.value.trim()) {
    validationError.value = 'Por favor ingresa tu correo electrónico y contraseña.'
    toast.error(validationError.value)
    return
  }

  const ok = await authStore.login({
    email: email.value.trim(),
    password: password.value
  })

  if (ok) {
    toast.success(`¡Bienvenido, ${authStore.usuario?.nombre || 'Usuario'}!`)
    router.push('/solicitudes')
  } else if (authStore.error) {
    toast.error(authStore.error)
  }
}
</script>

