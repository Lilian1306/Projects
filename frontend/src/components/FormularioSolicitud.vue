<template>
  <form @submit.prevent="handleSubmit" class="space-y-5">
    <div>
      <label class="block text-xs font-semibold text-slate-300 uppercase tracking-wider mb-1.5">Título</label>
      <input
        data-testid="form-titulo"
        v-model="form.titulo"
        type="text"
        required
        placeholder="Ej: Falla con la impresora de la oficina"
        class="w-full px-3.5 py-2.5 bg-slate-950 border border-slate-800 rounded-xl text-white text-sm placeholder-slate-500 focus:outline-none focus:border-brand-500"
      />
      <p v-if="errores.titulo" data-testid="error-titulo" class="text-red-400 text-xs mt-1">{{ errores.titulo }}</p>
    </div>

    <div>
      <label class="block text-xs font-semibold text-slate-300 uppercase tracking-wider mb-1.5">Descripción</label>
      <textarea
        data-testid="form-descripcion"
        v-model="form.descripcion"
        rows="4"
        required
        placeholder="Describa a detalle el inconveniente o requerimiento..."
        class="w-full px-3.5 py-2.5 bg-slate-950 border border-slate-800 rounded-xl text-white text-sm placeholder-slate-500 focus:outline-none focus:border-brand-500"
      ></textarea>
      <p v-if="errores.descripcion" data-testid="error-descripcion" class="text-red-400 text-xs mt-1">{{ errores.descripcion }}</p>
    </div>

    <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
      <div>
        <label class="block text-xs font-semibold text-slate-300 uppercase tracking-wider mb-1.5">Categoría</label>
        <select
          data-testid="form-categoria"
          v-model="form.categoriaId"
          required
          class="w-full px-3.5 py-2.5 bg-slate-950 border border-slate-800 rounded-xl text-white text-sm focus:outline-none focus:border-brand-500"
        >
          <option value="" disabled>Seleccione categoría</option>
          <option v-for="cat in categorias" :key="cat.id" :value="cat.id">
            {{ cat.nombre }} (SLA: {{ cat.slaHoras }}h)
          </option>
        </select>
        <p v-if="errores.categoria" data-testid="error-categoria" class="text-red-400 text-xs mt-1">{{ errores.categoria }}</p>
      </div>

      <div>
        <label class="block text-xs font-semibold text-slate-300 uppercase tracking-wider mb-1.5">Prioridad</label>
        <select
          data-testid="form-prioridad"
          v-model="form.prioridad"
          required
          class="w-full px-3.5 py-2.5 bg-slate-950 border border-slate-800 rounded-xl text-white text-sm focus:outline-none focus:border-brand-500"
        >
          <option value="Baja">Baja</option>
          <option value="Media">Media</option>
          <option value="Alta">Alta</option>
          <option value="Critica">Crítica</option>
        </select>
      </div>
    </div>

    <div v-if="errorGeneral" class="p-3 bg-red-500/10 border border-red-500/30 rounded-xl text-red-400 text-xs">
      {{ errorGeneral }}
    </div>

    <div class="flex items-center justify-end gap-3 pt-3 border-t border-slate-800">
      <button
        data-testid="form-cancelar"
        type="button"
        @click="$emit('cancelar')"
        class="px-4 py-2 text-xs font-semibold text-slate-300 hover:text-white bg-slate-800 hover:bg-slate-700 rounded-xl transition-colors"
      >
        Cancelar
      </button>
      <button
        data-testid="form-submit"
        type="submit"
        :disabled="enviando"
        class="px-5 py-2 text-xs font-semibold text-white bg-brand-600 hover:bg-brand-500 rounded-xl disabled:opacity-50 transition-all flex items-center gap-2"
      >
        <svg v-if="enviando" class="animate-spin h-4 w-4" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
        <span>{{ enviando ? 'Guardando...' : (modoEdicion ? 'Guardar Cambios' : 'Crear Solicitud') }}</span>
      </button>
    </div>
  </form>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import axios from 'axios'
import { toast } from 'vue3-toastify'
import { httpClient } from '@/api/httpClient'
import type { Categoria, PrioridadSolicitud } from '@/types'

const props = defineProps<{
  solicitudId?: string
  initialData?: {
    titulo: string
    descripcion: string
    categoriaId: string
    prioridad: PrioridadSolicitud
  }
}>()

const emit = defineEmits<{
  (e: 'guardado', id: string): void
  (e: 'cancelar'): void
}>()

const modoEdicion = !!props.solicitudId
const categorias = ref<Categoria[]>([])
const enviando = ref(false)
const errorGeneral = ref<string | null>(null)

const form = reactive({
  titulo: props.initialData?.titulo ?? '',
  descripcion: props.initialData?.descripcion ?? '',
  categoriaId: props.initialData?.categoriaId ?? '',
  prioridad: (props.initialData?.prioridad ?? 'Media') as PrioridadSolicitud
})

const errores = reactive({ titulo: '', descripcion: '', categoria: '' })

onMounted(async () => {
  const res = await httpClient.get<Categoria[]>('/categorias')
  categorias.value = res.data
  if (!modoEdicion && categorias.value.length > 0 && !form.categoriaId) {
    form.categoriaId = categorias.value[0].id
  }
})

function validar(): boolean {
  const titulo = form.titulo.trim()
  const descripcion = form.descripcion.trim()

  errores.titulo = titulo.length < 5 
    ? 'El título debe tener al menos 5 caracteres.' 
    : titulo.length > 120 
      ? 'El título no puede superar los 120 caracteres.' 
      : ''

  errores.descripcion = descripcion.length < 10 
    ? 'La descripción debe tener al menos 10 caracteres.' 
    : descripcion.length > 4000 
      ? 'La descripción no puede superar los 4000 caracteres.' 
      : ''

  errores.categoria = form.categoriaId ? '' : 'Seleccione una categoría.'

  return !errores.titulo && !errores.descripcion && !errores.categoria
}

async function handleSubmit() {
  if (!validar()) return
  enviando.value = true
  errorGeneral.value = null
  try {
    const payload = {
      titulo: form.titulo.trim(),
      descripcion: form.descripcion.trim(),
      categoriaId: form.categoriaId,
      prioridad: form.prioridad
    }
    if (modoEdicion) {
      await httpClient.put(`/solicitudes/${props.solicitudId}`, payload)
      toast.success('¡Solicitud actualizada exitosamente!')
      emit('guardado', props.solicitudId!)
    } else {
      const res = await httpClient.post<{ id: string }>('/solicitudes', payload)
      toast.success('¡Solicitud creada exitosamente!')
      emit('guardado', res.data.id)
    }
  } catch (err: unknown) {
    errorGeneral.value = (axios.isAxiosError<{ detail?: string }>(err) && err.response?.data?.detail)
      || 'Error al guardar la solicitud.'
    toast.error(errorGeneral.value)
  } finally {
    enviando.value = false
  }
}
</script>
