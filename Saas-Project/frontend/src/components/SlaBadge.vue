<template>
  <div 
    class="inline-flex items-center gap-1.5 px-2.5 py-1 text-xs font-semibold rounded-full border shadow-sm transition-all"
    :class="badgeClasses"
  >
    <svg v-if="slaVencido" class="w-3.5 h-3.5 animate-pulse text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
    </svg>
    <svg v-else class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
    </svg>
    <span>{{ labelText }}</span>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  slaFechaLimite: string
  slaVencido: boolean
  estado?: string
}>()

const badgeClasses = computed(() => {
  if (props.slaVencido && props.estado !== 'Resuelta' && props.estado !== 'Cerrada' && props.estado !== 'Cancelada') {
    return 'bg-red-500/15 text-red-400 border-red-500/30 font-bold'
  }

  return 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20'
})

const labelText = computed(() => {
  if (props.slaVencido && props.estado !== 'Resuelta' && props.estado !== 'Cerrada' && props.estado !== 'Cancelada') {
    return 'SLA VENCIDO'
  }
  
  if (!props.slaFechaLimite) return 'Sin SLA'

  const limite = new Date(props.slaFechaLimite)
  const ahora = new Date()
  const diffMs = limite.getTime() - ahora.getTime()

  if (diffMs <= 0) {
    return 'Vencido'
  }

  const diffHoras = Math.floor(diffMs / (1000 * 60 * 60))
  if (diffHoras < 24) {
    return `${diffHoras}h restantes`
  }

  const diffDias = Math.floor(diffHoras / 24)
  return `${diffDias}d restantes`
})
</script>
