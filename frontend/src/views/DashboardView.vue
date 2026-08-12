<template>
  <div class="space-y-6">
    <!-- Header / Título & Acción -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
      <div>
        <h1 class="text-2xl font-bold text-white tracking-tight">Mesa de Solicitudes</h1>
        <p class="text-slate-400 text-sm mt-1">Gestión integral de tickets de soporte y cumplimiento de SLA</p>
      </div>

      <router-link
        data-testid="btn-nueva-solicitud"
        to="/solicitudes/nueva"
        class="inline-flex items-center justify-center gap-2 px-4 py-2.5 bg-brand-600 hover:bg-brand-500 text-white font-semibold text-sm rounded-xl shadow-lg shadow-brand-600/30 transition-all hover:scale-[1.02]"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
        </svg>
        Nueva Solicitud
      </router-link>
    </div>

    <!-- Barra de Tarjetas Resumen / Stats -->
    <div class="grid grid-cols-2 lg:grid-cols-4 gap-4">
      <div class="bg-slate-900 border border-slate-800 rounded-2xl p-4 flex items-center gap-4">
        <div class="w-10 h-10 rounded-xl bg-blue-500/10 border border-blue-500/20 text-blue-400 flex items-center justify-center shrink-0">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
        </div>
        <div>
          <div class="text-xs font-semibold text-slate-400 uppercase tracking-wider">Total Tickets</div>
          <div class="text-xl font-bold text-white">{{ resultado?.total || 0 }}</div>
        </div>
      </div>

      <div class="bg-slate-900 border border-slate-800 rounded-2xl p-4 flex items-center gap-4">
        <div class="w-10 h-10 rounded-xl bg-red-500/10 border border-red-500/20 text-red-400 flex items-center justify-center shrink-0">
          <svg class="w-5 h-5 animate-pulse" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
          </svg>
        </div>
        <div>
          <div class="text-xs font-semibold text-slate-400 uppercase tracking-wider">SLA Vencidos</div>
          <div class="text-xl font-bold text-red-400">{{ totalSlaVencidos }}</div>
        </div>
      </div>

      <div class="bg-slate-900 border border-slate-800 rounded-2xl p-4 flex items-center gap-4">
        <div class="w-10 h-10 rounded-xl bg-purple-500/10 border border-purple-500/20 text-purple-400 flex items-center justify-center shrink-0">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
          </svg>
        </div>
        <div>
          <div class="text-xs font-semibold text-slate-400 uppercase tracking-wider">En Proceso</div>
          <div class="text-xl font-bold text-purple-300">{{ totalEnProceso }}</div>
        </div>
      </div>

      <div class="bg-slate-900 border border-slate-800 rounded-2xl p-4 flex items-center gap-4">
        <div class="w-10 h-10 rounded-xl bg-emerald-500/10 border border-emerald-500/20 text-emerald-400 flex items-center justify-center shrink-0">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
          </svg>
        </div>
        <div>
          <div class="text-xs font-semibold text-slate-400 uppercase tracking-wider">Resueltas</div>
          <div class="text-xl font-bold text-emerald-400">{{ totalResueltas }}</div>
        </div>
      </div>
    </div>

    <FiltrosSolicitudes @update-filtros="handleFiltrosUpdate" />

    <div class="bg-slate-900 border border-slate-800 rounded-2xl shadow-xl overflow-hidden">
      <div v-if="cargando" data-testid="listado-cargando" class="p-12 text-center text-slate-400 flex flex-col items-center gap-3">
        <svg class="animate-spin h-8 w-8 text-brand-500" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
        <span class="text-sm font-medium">Cargando solicitudes...</span>
      </div>

      <div v-else-if="!resultado?.items || resultado.items.length === 0" data-testid="listado-vacio" class="p-12 text-center text-slate-400">
        <svg class="w-12 h-12 mx-auto text-slate-600 mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
        </svg>
        <p class="font-semibold text-white">No se encontraron solicitudes</p>
        <p class="text-xs text-slate-500 mt-1">Pruebe ajustando los filtros o creando una nueva solicitud.</p>
      </div>

      <div v-else class="overflow-x-auto">
        <table class="w-full text-left text-sm text-slate-300">
          <thead class="bg-slate-950/80 text-xs font-semibold uppercase tracking-wider text-slate-400 border-b border-slate-800">
            <tr>
              <th class="py-3.5 px-4">Código</th>
              <th class="py-3.5 px-4">Título / Categoría</th>
              <th class="py-3.5 px-4">Agente</th>
              <th class="py-3.5 px-4">Prioridad</th>
              <th class="py-3.5 px-4">Estado</th>
              <th class="py-3.5 px-4">Estado SLA</th>
              <th class="py-3.5 px-4 text-right">Acción</th>
            </tr>
          </thead>
          <tbody data-testid="tabla-solicitudes" class="divide-y divide-slate-800/60">
            <tr 
              v-for="sol in resultado.items" 
              :key="sol.id"
              data-testid="fila-solicitud"
              :data-codigo="sol.codigo"
              class="hover:bg-slate-800/40 transition-colors group"
            >
              <td data-testid="celda-codigo" class="py-4 px-4 font-mono text-xs font-bold text-brand-400">
                {{ sol.codigo }}
              </td>

              <td class="py-4 px-4">
                <div class="font-semibold text-white group-hover:text-brand-300 transition-colors max-w-xs truncate">
                  {{ sol.titulo }}
                </div>
                <div class="text-xs text-slate-400 mt-0.5">
                  {{ sol.categoria?.nombre }}
                </div>
              </td>

              <td class="py-4 px-4 text-xs">
                <div class="text-slate-400 flex items-center gap-1 mt-0.5">
                  <span class="w-1.5 h-1.5 rounded-full" :class="sol.agente ? 'bg-blue-400' : 'bg-slate-600'"></span>
                  {{ sol.agente?.nombre || 'Sin asignar' }}
                </div>
              </td>

              <td data-testid="celda-prioridad" class="py-4 px-4">
                <span class="px-2.5 py-1 text-xs font-bold rounded-lg border uppercase tracking-wider" :class="getPrioridadClass(sol.prioridad)">
                  {{ sol.prioridad }}
                </span>
              </td>

              <td data-testid="celda-estado" class="py-4 px-4">
                <span class="px-2.5 py-1 text-xs font-semibold rounded-full border" :class="getEstadoClass(sol.estado)">
                  {{ getEstadoLabel(sol.estado) }}
                </span>
              </td>

              <td data-testid="celda-sla" class="py-4 px-4">
                <SlaBadge 
                  :sla-fecha-limite="sol.fechaLimiteSla" 
                  :sla-vencido="sol.vencida" 
                  :estado="sol.estado"
                />
                <span v-if="sol.vencida" data-testid="badge-vencida" class="sr-only">Vencida</span>
              </td>

              <td class="py-4 px-4 text-right">
                <!-- Si está asignado a otro agente y el usuario actual es Agente -->
                <div 
                  v-if="esDetalleDeshabilitado(sol)"
                  title="Este ticket está asignado a otro agente"
                  class="inline-block relative group/tooltip"
                >
                  <button
                    disabled
                    class="inline-flex items-center gap-1 px-3 py-1.5 bg-slate-800/40 text-slate-500 rounded-lg border border-slate-800/80 text-xs font-semibold cursor-not-allowed opacity-50"
                  >
                    Ver Detalle
                  </button>
  
                </div>

                <router-link 
                  v-else
                  :to="`/solicitudes/${sol.id}`"
                  class="inline-flex items-center gap-1 px-3 py-1.5 bg-slate-800 hover:bg-brand-600 text-slate-200 hover:text-white rounded-lg border border-slate-700 hover:border-brand-500 text-xs font-semibold transition-all shadow-sm"
                >
                  Ver Detalle
                </router-link>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div v-if="resultado && resultado.totalPaginas > 0" class="border-t border-slate-800 bg-slate-950/40">
        <Paginador 
          :pagina-actual="resultado.page"
          :total-paginas="resultado.totalPaginas"
          :total-registros="resultado.total"
          :tamano-pagina="resultado.pageSize"
          @change-page="handlePageChange"
        />
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'
import { httpClient } from '@/api/httpClient'
import { useAuthStore } from '@/stores/authStore'
import type { SolicitudResumen, ResultadoPaginado, SolicitudFiltros } from '@/types'
import FiltrosSolicitudes from '@/components/FiltrosSolicitudes.vue'
import SlaBadge from '@/components/SlaBadge.vue'
import Paginador from '@/components/Paginador.vue'

const authStore = useAuthStore()
const cargando = ref(false)
const resultado = ref<ResultadoPaginado<SolicitudResumen> | null>(null)

function esDetalleDeshabilitado(sol: SolicitudResumen): boolean {
  if (authStore.userRole !== 'Agente') return false
  return !!(sol.agente && sol.agente.id !== authStore.usuario?.id)
}

const filtros = ref<SolicitudFiltros>({
  page: 1,
  pageSize: 10
})

// Evita que una respuesta vieja (p. ej. de una tecla anterior en el buscador)
// sobrescriba el resultado de una petición más reciente al llegar fuera de orden.
let controladorPeticionActual: AbortController | null = null

const totalSlaVencidos = computed(() => {
  if (!resultado.value?.items) return 0
  return resultado.value.items.filter(s => s.vencida).length
})

const totalEnProceso = computed(() => {
  if (!resultado.value?.items) return 0
  return resultado.value.items.filter(s => s.estado === 'EnProceso' || s.estado === 'Asignada').length
})

const totalResueltas = computed(() => {
  if (!resultado.value?.items) return 0
  return resultado.value.items.filter(s => s.estado === 'Resuelta' || s.estado === 'Cerrada').length
})

onMounted(async () => {
  await cargarSolicitudes()
})

async function cargarSolicitudes() {
  controladorPeticionActual?.abort()
  const controlador = new AbortController()
  controladorPeticionActual = controlador

  cargando.value = true
  try {
    const res = await httpClient.get<ResultadoPaginado<SolicitudResumen>>('/solicitudes', {
      params: filtros.value,
      signal: controlador.signal
    })
    resultado.value = res.data
  } catch (err) {
    if (axios.isCancel(err)) return
    console.error('Error al cargar solicitudes', err)
  } finally {
    if (controladorPeticionActual === controlador) {
      cargando.value = false
    }
  }
}

function handleFiltrosUpdate(nuevosFiltros: SolicitudFiltros) {
  filtros.value = {
    ...filtros.value,
    ...nuevosFiltros,
    page: 1
  }
  cargarSolicitudes()
}

function handlePageChange(nuevaPagina: number) {
  filtros.value.page = nuevaPagina
  cargarSolicitudes()
}

const PRIORIDAD_CLASSES: Record<string, string> = {
  Critica: 'bg-red-500/15 text-red-400 border-red-500/30',
  Alta: 'bg-amber-500/15 text-amber-400 border-amber-500/30',
  Media: 'bg-blue-500/15 text-blue-400 border-blue-500/30',
  Baja: 'bg-emerald-500/15 text-emerald-400 border-emerald-500/30',
}

function getPrioridadClass(prioridad: string): string {
  return PRIORIDAD_CLASSES[prioridad] || 'bg-emerald-500/15 text-emerald-400 border-emerald-500/30'
}

function getEstadoClass(estado: string): string {
  const clases: Record<string, string> = {
    Nueva: 'bg-blue-500/10 text-blue-400 border-blue-500/30',
    Asignada: 'bg-purple-500/10 text-purple-400 border-purple-500/30',
    EnProceso: 'bg-indigo-500/10 text-indigo-300 border-indigo-500/30',
    Resuelta: 'bg-emerald-500/10 text-emerald-400 border-emerald-500/30',
    Cerrada: 'bg-slate-700/40 text-slate-400 border-slate-700',
  }

  return clases[estado] || 'bg-red-500/10 text-red-400 border-red-500/30'
}

function getEstadoLabel(estado: string): string {
  if (estado === 'EnProceso') return 'En Proceso'
  return estado
}
</script>
