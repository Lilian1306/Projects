<template>
  <div class="bg-slate-900 border border-slate-800 rounded-2xl p-4 sm:p-5 shadow-lg space-y-4">
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-4">
      <!-- Búsqueda por Texto -->
      <div>
        <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1.5">Buscar</label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-slate-500">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
          </div>
          <input
            data-testid="filtro-busqueda"
            v-model="q"
            type="text"
            placeholder="Código, título o descripción..."
            @input="emitFiltros"
            class="w-full pl-9 pr-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-sm placeholder-slate-500 focus:outline-none focus:border-brand-500 focus:ring-1 focus:ring-brand-500"
          />
        </div>
      </div>

      <!-- Filtro Estado -->
      <div>
        <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1.5">Estado</label>
        <select
          data-testid="filtro-estado"
          v-model="estado"
          @change="emitFiltros"
          class="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-sm focus:outline-none focus:border-brand-500 focus:ring-1 focus:ring-brand-500"
        >
          <option value="">Todos los Estados</option>
          <option value="Nueva">Nueva</option>
          <option value="Asignada">Asignada</option>
          <option value="EnProceso">En Proceso</option>
          <option value="Resuelta">Resuelta</option>
          <option value="Cerrada">Cerrada</option>
          <option value="Cancelada">Cancelada</option>
        </select>
      </div>

      <!-- Filtro Prioridad -->
      <div>
        <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1.5">Prioridad</label>
        <select
          data-testid="filtro-prioridad"
          v-model="prioridad"
          @change="emitFiltros"
          class="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-sm focus:outline-none focus:border-brand-500 focus:ring-1 focus:ring-brand-500"
        >
          <option value="">Todas las Prioridades</option>
          <option value="Baja">Baja</option>
          <option value="Media">Media</option>
          <option value="Alta">Alta</option>
          <option value="Critica">Crítica</option>
        </select>
      </div>

      <!-- Filtro Categoría -->
      <div>
        <label class="block text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1.5">Categoría</label>
        <select
          data-testid="filtro-categoria"
          v-model="categoriaId"
          @change="emitFiltros"
          class="w-full px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl text-white text-sm focus:outline-none focus:border-brand-500 focus:ring-1 focus:ring-brand-500"
        >
          <option value="">Todas las Categorías</option>
          <option v-for="cat in categorias" :key="cat.id" :value="cat.id">{{ cat.nombre }}</option>
        </select>
      </div>
    </div>

    <div class="flex items-center gap-3">
      <label class="flex items-center gap-2 px-3 py-2 bg-slate-950 border border-slate-800 rounded-xl cursor-pointer hover:border-slate-700 transition-colors">
        <input
          data-testid="filtro-vencidas"
          type="checkbox"
          v-model="vencidas"
          @change="emitFiltros"
          class="w-4 h-4 rounded bg-slate-900 border-slate-700 text-brand-600 focus:ring-brand-500 focus:ring-offset-slate-950"
        />
        <span class="text-xs font-medium text-red-400">Solo SLA Vencido</span>
      </label>

      <button
        data-testid="btn-limpiar-filtros"
        @click="limpiarFiltros"
        class="px-3 py-2 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded-xl border border-slate-700/60 transition-colors text-xs font-medium"
      >
        Limpiar
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { httpClient } from '@/api/httpClient'
import type { SolicitudFiltros, Categoria } from '@/types'

const emit = defineEmits<{
  (e: 'update-filtros', filtros: SolicitudFiltros): void
}>()

const q = ref('')
const estado = ref('')
const prioridad = ref('')
const categoriaId = ref('')
const vencidas = ref(false)
const categorias = ref<Categoria[]>([])

onMounted(async () => {
  try {
    const res = await httpClient.get<Categoria[]>('/categorias')
    categorias.value = res.data
  } catch {
    // silencioso — el filtro funciona sin categorías
  }
})

function emitFiltros() {
  emit('update-filtros', {
    q: q.value.trim() || undefined,
    estado: estado.value || undefined,
    prioridad: prioridad.value || undefined,
    categoriaId: categoriaId.value || undefined,
    vencidas: vencidas.value || undefined,
    page: 1
  })
}

function limpiarFiltros() {
  q.value = ''
  estado.value = ''
  prioridad.value = ''
  categoriaId.value = ''
  vencidas.value = false
  emitFiltros()
}
</script>
