<template>
  <div class="bg-slate-900 border border-slate-800 rounded-2xl p-5 shadow-xl space-y-4">
    <h3 class="text-sm font-bold text-white uppercase tracking-wider flex items-center gap-2">
      <svg class="w-4 h-4 text-brand-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
      </svg>
      Acciones de Gestión
    </h3>

    <div v-if="errorMsg" data-testid="modal-error" class="p-3 bg-red-500/10 border border-red-500/30 rounded-xl text-red-400 text-xs">
      {{ errorMsg }}
    </div>

    <div class="flex flex-wrap items-center gap-3">
      <button
        v-if="canAsignar"
        data-testid="btn-accion-asignar"
        @click="handleAsignar"
        :disabled="ejecutando"
        class="px-4 py-2 bg-purple-600 hover:bg-purple-500 text-white font-semibold text-xs rounded-xl shadow-lg shadow-purple-600/30 transition-all disabled:opacity-50 flex items-center gap-1.5"
      >
        {{ rol === 'Agente' ? 'Asignarme Ticket' : (props.estado === 'Nueva' ? 'Asignar' : 'Reasignar Ticket') }}
      </button>

      <button
        v-if="canIniciar"
        data-testid="btn-accion-iniciar"
        @click="ejecutar('iniciar')"
        :disabled="ejecutando"
        class="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white font-semibold text-xs rounded-xl shadow-lg shadow-indigo-600/30 transition-all disabled:opacity-50 flex items-center gap-1.5"
      >
        Iniciar Trabajo
      </button>

      <button
        v-if="canResolver"
        data-testid="btn-accion-resolver"
        @click="showResolverModal = true"
        :disabled="ejecutando"
        class="px-4 py-2 bg-emerald-600 hover:bg-emerald-500 text-white font-semibold text-xs rounded-xl shadow-lg shadow-emerald-600/30 transition-all disabled:opacity-50 flex items-center gap-1.5"
      >
        Resolver
      </button>

      <button
        v-if="canReabrir"
        data-testid="btn-accion-reabrir"
        @click="ejecutar('reabrir')"
        :disabled="ejecutando"
        class="px-4 py-2 bg-amber-600 hover:bg-amber-500 text-white font-semibold text-xs rounded-xl shadow-lg shadow-amber-600/30 transition-all disabled:opacity-50 flex items-center gap-1.5"
      >
        Reabrir
      </button>

      <button
        v-if="canCerrar"
        data-testid="btn-accion-cerrar"
        @click="ejecutar('cerrar')"
        :disabled="ejecutando"
        class="px-4 py-2 bg-slate-700 hover:bg-slate-600 text-slate-200 font-semibold text-xs rounded-xl border border-slate-600 transition-all disabled:opacity-50 flex items-center gap-1.5"
      >
        Cerrar Ticket
      </button>

      <button
        v-if="canCancelar"
        data-testid="btn-accion-cancelar"
        @click="showCancelarModal = true"
        :disabled="ejecutando"
        class="px-4 py-2 bg-red-600/20 hover:bg-red-600/30 text-red-400 font-semibold text-xs rounded-xl border border-red-500/30 transition-all disabled:opacity-50 flex items-center gap-1.5 ml-auto"
      >
        Cancelar Solicitud
      </button>
    </div>

    <!-- Modal Asignar (Solo visible para Admin) -->
    <div v-if="showAsignarModal && rol === 'Admin'" data-testid="modal-accion" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-950/80 backdrop-blur-sm">
      <div class="bg-slate-900 border border-slate-800 rounded-2xl max-w-md w-full p-6 space-y-4">
        <h4 class="font-bold text-white text-base">Asignar Agente</h4>
        <p class="text-xs text-slate-400">Seleccione un agente de la lista o ingrese su ID manual (GUID).</p>
        
        <!-- Select Desplegable de Agentes -->
        <div class="space-y-1">
          <label class="block text-[10px] font-bold text-slate-400 uppercase tracking-wider">Seleccionar Agente de la Empresa</label>
          <select
            data-testid="modal-select-agente"
            v-model="agenteIdInput"
            :disabled="cargandoAgentes"
            class="w-full p-3 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-brand-500 disabled:opacity-50"
          >
            <option value="">
              {{ cargandoAgentes ? '-- Cargando agentes... --' : '-- Seleccione un Agente de la lista --' }}
            </option>
            <option v-for="agente in agentes" :key="agente.id" :value="agente.id">
              {{ agente.nombre }} ({{ agente.email }}) — GUID: {{ agente.id }}
            </option>
          </select>
        </div>

        <!-- Confirmación visual de GUID Seleccionado -->
        <div v-if="agenteIdInput" class="p-2.5 bg-purple-500/10 border border-purple-500/20 rounded-xl flex items-center justify-between gap-2 text-xs">
          <span class="text-[10px] font-bold text-purple-400 uppercase tracking-wider">GUID Seleccionado:</span>
          <code class="font-mono text-[11px] text-purple-200 select-all truncate">{{ agenteIdInput }}</code>
        </div>

        <!-- Entrada manual GUID -->
        <div class="space-y-1">
          <label class="block text-[10px] font-bold text-slate-400 uppercase tracking-wider">O ingrese ID del Agente (GUID)</label>
          <input
            v-model="agenteIdInput"
            type="text"
            placeholder="ID del agente (GUID)"
            class="w-full p-3 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-brand-500 font-mono"
          />
        </div>

        <div class="flex justify-end gap-2 pt-2">
          <button data-testid="modal-cancelar" @click="showAsignarModal = false" class="px-3 py-1.5 bg-slate-800 text-slate-300 text-xs rounded-lg">Cancelar</button>
          <button data-testid="modal-confirmar" @click="confirmarAsignar" :disabled="!agenteIdInput.trim()" class="px-4 py-1.5 bg-purple-600 text-white text-xs font-semibold rounded-lg disabled:opacity-50">Asignar</button>
        </div>
      </div>
    </div>

    <!-- Modal Resolver -->
    <div v-if="showResolverModal" data-testid="modal-accion" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-950/80 backdrop-blur-sm">
      <div class="bg-slate-900 border border-slate-800 rounded-2xl max-w-md w-full p-6 space-y-4">
        <h4 class="font-bold text-white text-base">Motivo de Resolución</h4>
        <p class="text-xs text-slate-400">Mínimo 20 caracteres.</p>
        <textarea
          data-testid="modal-motivo"
          v-model="motivo"
          rows="4"
          placeholder="Describa la solución aplicada..."
          class="w-full p-3 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-brand-500"
        ></textarea>
        <div class="flex justify-end gap-2">
          <button data-testid="modal-cancelar" @click="showResolverModal = false" class="px-3 py-1.5 bg-slate-800 text-slate-300 text-xs rounded-lg">Cancelar</button>
          <button data-testid="modal-confirmar" @click="confirmarResolver" :disabled="motivo.trim().length < 20" class="px-4 py-1.5 bg-emerald-600 text-white text-xs font-semibold rounded-lg disabled:opacity-50">Resolver</button>
        </div>
      </div>
    </div>

    <!-- Modal Cancelar -->
    <div v-if="showCancelarModal" data-testid="modal-accion" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-950/80 backdrop-blur-sm">
      <div class="bg-slate-900 border border-slate-800 rounded-2xl max-w-md w-full p-6 space-y-4">
        <h4 class="font-bold text-white text-base">Motivo de Cancelación</h4>
        <p class="text-xs text-slate-400">Mínimo 10 caracteres.</p>
        <textarea
          data-testid="modal-motivo"
          v-model="motivo"
          rows="3"
          placeholder="Motivo de cancelación..."
          class="w-full p-3 bg-slate-950 border border-slate-800 rounded-xl text-white text-xs focus:outline-none focus:border-brand-500"
        ></textarea>
        <div class="flex justify-end gap-2">
          <button data-testid="modal-cancelar" @click="showCancelarModal = false" class="px-3 py-1.5 bg-slate-800 text-slate-300 text-xs rounded-lg">Volver</button>
          <button data-testid="modal-confirmar" @click="confirmarCancelar" :disabled="motivo.trim().length < 10" class="px-4 py-1.5 bg-red-600 text-white text-xs font-semibold rounded-lg disabled:opacity-50">Confirmar</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { toast } from 'vue3-toastify'
import { httpClient } from '@/api/httpClient'
import { useAuthStore } from '@/stores/authStore'
import type { EstadoSolicitud } from '@/types'

interface AgenteOption {
  id: string
  nombre: string
  email: string
}

const props = defineProps<{
  solicitudId: string
  estado: EstadoSolicitud
  solicitanteId: string
  agenteId?: string
}>()

const emit = defineEmits<{
  (e: 'updated'): void
}>()

const authStore = useAuthStore()
const ejecutando = ref(false)
const errorMsg = ref<string | null>(null)

const showAsignarModal = ref(false)
const showResolverModal = ref(false)
const showCancelarModal = ref(false)
const motivo = ref('')
const agenteIdInput = ref('')

const agentes = ref<AgenteOption[]>([])
const cargandoAgentes = ref(false)

async function abrirModalAsignar() {
  showAsignarModal.value = true
  cargandoAgentes.value = true
  try {
    const res = await httpClient.get<AgenteOption[]>('/usuarios/agentes')
    agentes.value = res.data
  } catch (err) {
    console.error('Error al cargar agentes:', err)
  } finally {
    cargandoAgentes.value = false
  }
}

function handleAsignar() {
  if (rol.value === 'Admin') {
    abrirModalAsignar()
  } else if (authStore.usuario?.id) {
    // Para el rol Agente, se asigna directamente a sí mismo sin mostrar el dropdown con GUIDs
    ejecutar('asignar', authStore.usuario.id)
  }
}

const rol = computed(() => authStore.userRole)
const esAgenteOAdmin = computed(() => rol.value === 'Agente' || rol.value === 'Admin')
const esSolicitantePropietario = computed(() =>
  rol.value === 'Solicitante' && authStore.usuario?.id === props.solicitanteId
)

// RN-02 + RN-03
const canAsignar = computed(() => {
  if (props.estado !== 'Nueva' && props.estado !== 'Asignada' && props.estado !== 'EnProceso') return false

  if (rol.value === 'Admin') return true

  if (rol.value === 'Agente') {
    // Un agente solo puede asignarse un ticket si está 'Nueva' (disponible/sin asignar)
    return props.estado === 'Nueva'
  }

  return false
})
const canIniciar  = computed(() => props.estado === 'Asignada' && props.agenteId === authStore.usuario?.id)
const canResolver = computed(() => esAgenteOAdmin.value && props.estado === 'EnProceso')
const canReabrir  = computed(() => esAgenteOAdmin.value && props.estado === 'Resuelta')
const canCerrar   = computed(() => {
  if (props.estado !== 'Resuelta') return false
  return rol.value === 'Admin' || rol.value === 'Agente' || esSolicitantePropietario.value
})
const canCancelar = computed(() => rol.value === 'Admin' && (props.estado === 'Nueva' || props.estado === 'Asignada' || props.estado === 'EnProceso'))

const MSJ_ACCIONES: Record<string, string> = {
  asignar: 'Solicitud asignada exitosamente.',
  iniciar: 'Trabajo iniciado en la solicitud.',
  resolver: 'Solicitud marcada como resuelta.',
  reabrir: 'Solicitud reabierta correctamente.',
  cerrar: 'Solicitud cerrada.',
  cancelar: 'Solicitud cancelada.'
}

async function ejecutar(accion: string, agenteId?: string, motivoTexto?: string) {
  ejecutando.value = true
  errorMsg.value = null
  try {
    await httpClient.post(`/solicitudes/${props.solicitudId}/transiciones`, {
      accion,
      ...(agenteId ? { agenteId } : {}),
      ...(motivoTexto ? { motivo: motivoTexto } : {})
    })
    toast.success(MSJ_ACCIONES[accion] || 'Acción ejecutada exitosamente.')
    emit('updated')
  } catch (err: unknown) {
    const e = err as { response?: { data?: { detail?: string } } }
    errorMsg.value = e.response?.data?.detail || 'Error al ejecutar la acción.'
    toast.error(errorMsg.value)
  } finally {
    ejecutando.value = false
  }
}

function confirmarAsignar() {
  showAsignarModal.value = false
  ejecutar('asignar', agenteIdInput.value.trim())
  agenteIdInput.value = ''
}

function confirmarResolver() {
  showResolverModal.value = false
  ejecutar('resolver', undefined, motivo.value.trim())
  motivo.value = ''
}

function confirmarCancelar() {
  showCancelarModal.value = false
  ejecutar('cancelar', undefined, motivo.value.trim())
  motivo.value = ''
}
</script>
