<template>
  <div v-if="totalPaginas > 1" class="flex flex-col sm:flex-row items-center justify-between gap-4 py-4 px-2">
    <div class="text-xs text-slate-400">
      Mostrando <span class="font-semibold text-white">{{ inicioRegistro }}</span> a <span class="font-semibold text-white">{{ finRegistro }}</span> de <span class="font-semibold text-white">{{ totalRegistros }}</span> resultados
    </div>

    <div class="flex items-center gap-2">
      <button
        data-testid="paginacion-anterior"
        @click="$emit('change-page', paginaActual - 1)"
        :disabled="paginaActual <= 1"
        class="px-3 py-1.5 text-xs font-medium bg-slate-800 hover:bg-slate-700 disabled:opacity-40 disabled:cursor-not-allowed text-slate-200 rounded-lg border border-slate-700/60 transition-colors flex items-center gap-1"
      >
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
        </svg>
        Anterior
      </button>

      <span data-testid="paginacion-info" class="text-xs font-semibold px-3 py-1.5 bg-slate-900 text-slate-300 rounded-lg border border-slate-800">
        Página {{ paginaActual }} de {{ totalPaginas }} — {{ totalRegistros }} resultados
      </span>

      <button
        data-testid="paginacion-siguiente"
        @click="$emit('change-page', paginaActual + 1)"
        :disabled="paginaActual >= totalPaginas"
        class="px-3 py-1.5 text-xs font-medium bg-slate-800 hover:bg-slate-700 disabled:opacity-40 disabled:cursor-not-allowed text-slate-200 rounded-lg border border-slate-700/60 transition-colors flex items-center gap-1"
      >
        Siguiente
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
        </svg>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  paginaActual: number
  totalPaginas: number
  totalRegistros: number
  tamanoPagina: number
}>()

defineEmits<{
  (e: 'change-page', page: number): void
}>()

const inicioRegistro = computed(() => {
  if (props.totalRegistros === 0) return 0
  return (props.paginaActual - 1) * props.tamanoPagina + 1
})

const finRegistro = computed(() => {
  return Math.min(props.paginaActual * props.tamanoPagina, props.totalRegistros)
})
</script>
