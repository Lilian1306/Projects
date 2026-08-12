<template>
  <div class="max-w-2xl mx-auto space-y-6">
    <div>
      <router-link to="/solicitudes" class="inline-flex items-center gap-1.5 text-xs font-semibold text-slate-400 hover:text-white transition-colors mb-4">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
        </svg>
        Volver a Solicitudes
      </router-link>
      <h1 class="text-2xl font-bold text-white">Nueva Solicitud</h1>
    </div>

    <div class="bg-slate-900 border border-slate-800 rounded-2xl p-6 shadow-xl">
      <FormularioSolicitud
        @guardado="(id) => router.push(`/solicitudes/${id}`)"
        @cancelar="router.push('/solicitudes')"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import FormularioSolicitud from '@/components/FormularioSolicitud.vue'

const router = useRouter()
</script>
