export type EstadoSolicitud = 'Nueva' | 'Asignada' | 'EnProceso' | 'Resuelta' | 'Cerrada' | 'Cancelada'
export type PrioridadSolicitud = 'Baja' | 'Media' | 'Alta' | 'Critica'

export interface ResumenRef {
  id: string
  nombre: string
}

export interface SolicitudResumen {
  id: string
  codigo: string
  titulo: string
  prioridad: PrioridadSolicitud
  estado: EstadoSolicitud
  categoria: ResumenRef
  agente: ResumenRef | null
  fechaCreacion: string
  fechaLimiteSla: string
  vencida: boolean
}

export interface SolicitudDetalle {
  id: string
  codigo: string
  titulo: string
  descripcion: string
  categoria: ResumenRef
  prioridad: PrioridadSolicitud
  estado: EstadoSolicitud
  solicitante: ResumenRef
  agente: ResumenRef | null
  fechaCreacion: string
  fechaLimiteSla: string
  vencida: boolean
  motivoResolucion?: string
  motivoCancelacion?: string
  fechaResolucion?: string
}

export interface CrearSolicitudPayload {
  titulo: string
  descripcion: string
  categoriaId: string
  prioridad: PrioridadSolicitud
}

export interface EditarSolicitudPayload {
  titulo: string
  descripcion: string
  categoriaId: string
  prioridad: PrioridadSolicitud
}

export interface EjecutarTransicionPayload {
  accion: string
  agenteId?: string
  motivo?: string
}

export interface SolicitudFiltros {
  q?: string
  estado?: string
  prioridad?: string
  categoriaId?: string
  vencidas?: boolean
  page?: number
  pageSize?: number
  sort?: string
}

export interface ResultadoPaginado<T> {
  items: T[]
  page: number
  pageSize: number
  total: number
  totalPaginas: number
}
