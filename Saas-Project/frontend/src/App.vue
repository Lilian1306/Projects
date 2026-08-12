<template>
  <div class="min-h-screen bg-gray-50 text-gray-900">
    <router-view />
    <!-- Toast global requerido por data-testid -->
    <div
      v-if="toastMsg"
      data-testid="toast-mensaje"
      class="fixed bottom-6 right-6 z-50 px-5 py-3 bg-slate-800 border border-slate-700 text-white text-sm rounded-xl shadow-2xl"
    >
      {{ toastMsg }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, provide } from 'vue'

const toastMsg = ref<string | null>(null)
let toastTimer: ReturnType<typeof setTimeout> | null = null

function showToast(msg: string) {
  toastMsg.value = msg
  if (toastTimer) clearTimeout(toastTimer)
  toastTimer = setTimeout(() => { toastMsg.value = null }, 3000)
}

provide('showToast', showToast)
</script>
