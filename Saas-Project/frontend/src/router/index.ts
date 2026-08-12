import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import LoginView from '@/views/LoginView.vue'
import MainLayout from '@/layouts/MainLayout.vue'
import DashboardView from '@/views/DashboardView.vue'
import SolicitudDetalleView from '@/views/SolicitudDetalleView.vue'
import NuevaSolicitudView from '@/views/NuevaSolicitudView.vue'
import EditarSolicitudView from '@/views/EditarSolicitudView.vue'

const routes: Array<RouteRecordRaw> = [
  {
    path: '/login',
    name: 'Login',
    component: LoginView,
    meta: { guestOnly: true }
  },
  {
    path: '/',
    component: MainLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        redirect: '/solicitudes'
      },
      {
        path: 'solicitudes',
        name: 'Dashboard',
        component: DashboardView
      },
      {
        path: 'solicitudes/nueva',
        name: 'SolicitudNueva',
        component: NuevaSolicitudView
      },
      {
        path: 'solicitudes/:id',
        name: 'SolicitudDetalle',
        component: SolicitudDetalleView
      },
      {
        path: 'solicitudes/:id/editar',
        name: 'SolicitudEditar',
        component: EditarSolicitudView
      }
    ]
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/solicitudes'
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, _from, next) => {
  const authStore = useAuthStore()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
  } else if (to.meta.guestOnly && authStore.isAuthenticated) {
    next('/solicitudes')
  } else {
    next()
  }
})

export default router
