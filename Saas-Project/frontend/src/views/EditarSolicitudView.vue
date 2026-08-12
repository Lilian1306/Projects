<template>
  <div class="max-w-2xl mx-auto space-y-6">
    <div>
      <router-link :to="`/solicitudes/${route.params.id}`" class="inline-flex items-center gap-1.5 text-xs font-semibold text-slate-400 hover:text-white transition-colors mb-4">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
        </svg>
        Volver al Detalle
      </router-link>
      <h1 class="text-2xl font-bold text-white">Editar Solicitud</h1>
    </div>

    <div v-if="cargando" class="p-12 text-center text-slate-400">
      <svg class="animate-spin h-8 w-8 text-brand-500 mx-auto" fill="none" viewBox="0 0 24 24">
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
      </svg>
    </div>

    <div v-else-if="errorMsg" class="p-6 bg-red-500/10 border border-red-500/30 rounded-2xl text-red-400 text-sm">
      {{ errorMsg }}
    </div>

    <div v-else-if="initialData" class="bg-slate-900 border border-slate-800 rounded-2xl p-6 shadow-xl">
      <FormularioSolicitud
        :solicitud-id="String(route.params.id)"
        :initial-data="initialData"
        @guardado="(id) => router.push(`/solicitudes/${id}`)"
        @cancelar="router.push(`/solicitudes/${route.params.id}`)"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'
import { httpClient } from '@/api/httpClient'
import type { SolicitudDetalle, PrioridadSolicitud } from '@/types'
import FormularioSolicitud from '@/components/FormularioSolicitud.vue'

const route = useRoute()
const router = useRouter()
const cargando = ref(true)
const errorMsg = ref<string | null>(null)
const initialData = ref<{ titulo: string; descripcion: string; categoriaId: string; prioridad: PrioridadSolicitud } | null>(null)

onMounted(async () => {
  try {
    const res = await httpClient.get<SolicitudDetalle>(`/solicitudes/${route.params.id}`)
    const s = res.data
    initialData.value = {
      titulo: s.titulo,
      descripcion: s.descripcion,
      categoriaId: s.categoria.id,
      prioridad: s.prioridad
    }
  } catch (err: unknown) {
    errorMsg.value = (axios.isAxiosError<{ detail?: string }>(err) && err.response?.data?.detail)
      || 'No se pudo cargar la solicitud.'
  } finally {
    cargando.value = false
  }
})
</script>
