<template>
  <div class="space-y-6 max-w-5xl mx-auto">
    <!-- Botón Volver & Header -->
    <div>
      <router-link 
        to="/solicitudes" 
        class="inline-flex items-center gap-1.5 text-xs font-semibold text-slate-400 hover:text-white transition-colors mb-4"
      >
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
        </svg>
        Volver a Solicitudes
      </router-link>

      <!-- Carga / Error -->
      <div v-if="cargando" class="p-12 text-center text-slate-400">
        <svg class="animate-spin h-8 w-8 text-brand-500 mx-auto mb-3" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
        Cargando detalle de la solicitud...
      </div>

      <div v-else-if="errorMsg" class="p-8 bg-red-500/10 border border-red-500/30 rounded-2xl text-center text-red-400 text-sm">
        {{ errorMsg }}
      </div>

      <div v-else-if="solicitud" class="space-y-6">
        <!-- Título y Badges -->
        <div class="bg-slate-900 border border-slate-800 rounded-2xl p-6 shadow-xl space-y-4">
          <div class="flex flex-wrap items-center justify-between gap-3">
            <span data-testid="detalle-codigo" class="font-mono text-sm font-bold text-brand-400 bg-brand-500/10 border border-brand-500/20 px-3 py-1 rounded-lg">
              {{ solicitud.codigo }}
            </span>

            <div class="flex items-center gap-2">
              <span data-testid="detalle-prioridad" class="px-3 py-1 text-xs font-bold rounded-lg border uppercase tracking-wider" :class="getPrioridadClass(solicitud.prioridad)">
                {{ solicitud.prioridad }}
              </span>

              <span data-testid="detalle-estado" class="px-3 py-1 text-xs font-semibold rounded-full border" :class="getEstadoClass(solicitud.estado)">
                {{ getEstadoLabel(solicitud.estado) }}
              </span>

              <SlaBadge 
                data-testid="detalle-vencida"
                :sla-fecha-limite="solicitud.fechaLimiteSla" 
                :sla-vencido="solicitud.vencida" 
                :estado="solicitud.estado"
              />
            </div>
          </div>

          <h1 data-testid="detalle-titulo" class="text-2xl font-bold text-white leading-snug">
            {{ solicitud.titulo }}
          </h1>
        </div>

        <!-- Grid Información General -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <div class="bg-slate-900 border border-slate-800 rounded-2xl p-4">
            <div class="text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">Categoría</div>
            <div data-testid="detalle-categoria" class="text-sm font-semibold text-white">{{ solicitud.categoria?.nombre }}</div>
          </div>

          <div class="bg-slate-900 border border-slate-800 rounded-2xl p-4">
            <div class="text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">Solicitante</div>
            <div class="text-sm font-semibold text-white">{{ solicitud.solicitante?.nombre }}</div>
          </div>

          <div class="bg-slate-900 border border-slate-800 rounded-2xl p-4">
            <div class="text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">Agente Asignado</div>
            <div data-testid="detalle-agente" class="text-sm font-semibold" :class="solicitud.agente ? 'text-white' : 'text-slate-500 italic'">
              {{ solicitud.agente?.nombre || 'Sin agente asignado' }}
            </div>
          </div>

          <div class="bg-slate-900 border border-slate-800 rounded-2xl p-4">
            <div class="text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">Fecha Límite SLA</div>
            <div data-testid="detalle-fecha-limite" class="text-xs font-medium text-slate-300">{{ formatFecha(solicitud.fechaLimiteSla) }}</div>
          </div>

          <div class="bg-slate-900 border border-slate-800 rounded-2xl p-4">
            <div class="text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">Fecha de Creación</div>
            <div data-testid="detalle-fecha-creacion" class="text-xs font-medium text-slate-300">{{ formatFecha(solicitud.fechaCreacion) }}</div>
          </div>
        </div>

        <!-- Descripción -->
        <div class="bg-slate-900 border border-slate-800 rounded-2xl p-6 shadow-xl space-y-2">
          <h3 class="text-xs font-semibold text-slate-400 uppercase tracking-wider">Descripción del Inconveniente</h3>
          <p data-testid="detalle-descripcion" class="text-slate-200 text-sm leading-relaxed whitespace-pre-line">
            {{ solicitud.descripcion }}
          </p>
        </div>

        <!-- Notas de Resolución (si aplica) -->
        <div v-if="solicitud.motivoResolucion" class="bg-emerald-500/10 border border-emerald-500/30 rounded-2xl p-6 shadow-xl space-y-2">
          <h3 class="text-xs font-bold text-emerald-400 uppercase tracking-wider flex items-center gap-2">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            Notas de Resolución
          </h3>
          <p data-testid="detalle-motivo" class="text-emerald-200 text-sm whitespace-pre-line">
            {{ solicitud.motivoResolucion }}
          </p>
          <div v-if="solicitud.fechaResolucion" class="text-xs text-emerald-400/80 pt-2 border-t border-emerald-500/20">
            Resuelto el: {{ formatFecha(solicitud.fechaResolucion) }}
          </div>
        </div>

        <!-- Motivo Cancelación (si aplica) -->
        <div v-if="solicitud.motivoCancelacion" class="bg-red-500/10 border border-red-500/30 rounded-2xl p-6 shadow-xl space-y-2">
          <h3 class="text-xs font-bold text-red-400 uppercase tracking-wider flex items-center gap-2">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
            Motivo de Cancelación
          </h3>
          <p data-testid="detalle-motivo" class="text-red-200 text-sm whitespace-pre-line">
            {{ solicitud.motivoCancelacion }}
          </p>
        </div>

        <!-- Botón Editar + Panel Transiciones -->
        <div class="flex justify-end">
          <router-link
            v-if="puedeEditar"
            data-testid="btn-editar"
            :to="`/solicitudes/${solicitud.id}/editar`"
            class="inline-flex items-center gap-2 px-4 py-2 bg-slate-800 hover:bg-slate-700 text-slate-200 text-xs font-semibold rounded-xl border border-slate-700 transition-colors"
          >
            Editar Solicitud
          </router-link>
        </div>

        <PanelTransiciones 
          :solicitud-id="solicitud.id" 
          :estado="solicitud.estado"
          :solicitante-id="solicitud.solicitante.id"
          :agente-id="solicitud.agente?.id"
          @updated="cargarDetalle"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import axios from 'axios'
import { httpClient } from '@/api/httpClient'
import { useAuthStore } from '@/stores/authStore'
import type { SolicitudDetalle } from '@/types'
import SlaBadge from '@/components/SlaBadge.vue'
import PanelTransiciones from '@/components/PanelTransiciones.vue'

const route = useRoute()
const authStore = useAuthStore()
const solicitud = ref<SolicitudDetalle | null>(null)
const cargando = ref(true)
const errorMsg = ref<string | null>(null)

const puedeEditar = computed(() => {
  if (!solicitud.value) return false
  if (authStore.userRole === 'Admin' || authStore.userRole === 'Agente') return solicitud.value.estado === 'Nueva'
  // Solicitante: solo las propias y solo en Nueva
  return authStore.userRole === 'Solicitante' &&
    solicitud.value.solicitante.id === authStore.usuario?.id &&
    solicitud.value.estado === 'Nueva'
})

onMounted(async () => {
  await cargarDetalle()
})

async function cargarDetalle() {
  const id = route.params.id as string
  if (!id) return

  cargando.value = true
  errorMsg.value = null

  try {
    const res = await httpClient.get<SolicitudDetalle>(`/solicitudes/${id}`)
    solicitud.value = res.data
  } catch (err: unknown) {
    errorMsg.value = (axios.isAxiosError<{ detail?: string }>(err) && err.response?.data?.detail)
      || 'No se pudo cargar el detalle de la solicitud.'
  } finally {
    cargando.value = false
  }
}

function formatFecha(fechaIso: string): string {
  if (!fechaIso) return '-'
  const d = new Date(fechaIso)
  return d.toLocaleString('es-ES', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const PRIORIDAD_CLASSES: Record<string, string> = {
  Critica: 'bg-red-500/15 text-red-400 border-red-500/30',
  Alta: 'bg-amber-500/15 text-amber-400 border-amber-500/30',
  Media: 'bg-blue-500/15 text-blue-400 border-blue-500/30',
  Baja: 'bg-emerald-500/15 text-emerald-400 border-emerald-500/30',
}

const ESTADO_CLASSES: Record<string, string> = {
  Nueva: 'bg-blue-500/10 text-blue-400 border-blue-500/30',
  Asignada: 'bg-purple-500/10 text-purple-400 border-purple-500/30',
  EnProceso: 'bg-indigo-500/10 text-indigo-300 border-indigo-500/30',
  Resuelta: 'bg-emerald-500/10 text-emerald-400 border-emerald-500/30',
  Cerrada: 'bg-slate-700/40 text-slate-400 border-slate-700',
}

function getPrioridadClass(prioridad: string): string {
  return PRIORIDAD_CLASSES[prioridad] || 'bg-emerald-500/15 text-emerald-400 border-emerald-500/30'
}

function getEstadoClass(estado: string): string {
  return ESTADO_CLASSES[estado] || 'bg-red-500/10 text-red-400 border-red-500/30'
}

function getEstadoLabel(estado: string): string {
  if (estado === 'EnProceso') return 'En Proceso'
  return estado
}
</script>
