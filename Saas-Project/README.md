# MesaSitec — Sistema de Mesa de Ayuda Multi-Tenant

## Requisitos Previos

| Herramienta | Versión mínima |
| --- | --- |
| .NET SDK | 8.0 (soporta hasta .NET 10 via `RollForward`) |
| Node.js | 18.x o superior |
| npm | 9.x o superior |
| Docker (Opcional) | 20.x o superior |

---

## Levantar el proyecto (4 comandos)

### 1. Clonar e instalar dependencias del frontend
```bash
cd frontend && npm install
```

### 2. Iniciar el backend (migra y siembra automáticamente)
```bash
cd backend && dotnet run --project src/MesaSitec.Api
```

### 3. Iniciar el frontend (en otra terminal)
```bash
cd frontend && npm run dev
```

### 4. Verificar que todo funciona
```
API:      http://localhost:5080/health   → { "estado": "ok" }
Swagger:  http://localhost:5080/swagger
Frontend: http://localhost:5173
```

> La base de datos SQLite se crea y siembra automáticamente al primer arranque. No se requiere ningún paso manual.

---

## Despliegue en Producción / Nube (Vercel + Render)

- **Frontend (Vue 3)**: Desplegado en **Vercel** con reglas de enrutamiento SPA (`frontend/vercel.json`).
- **Backend (.NET 8)**: Desplegado en **Render** mediante contenedores Docker (`backend/Dockerfile`).
- **Variable de Entorno**: `VITE_API_URL=https://<tu-api-en-render>.onrender.com/api/v1`

> **Nota sobre Persistencia de Datos en Producción**:  
> Al utilizar **SQLite** como base de datos local dentro del contenedor Docker en el plan gratuito de **Render**, el sistema utiliza almacenamiento efímero (temporal). Tras 15 minutos de inactividad, Render suspende el contenedor (*sleep*); al volver a despertar o reiniciar, los datos creados durante esa sesión se restablecen automáticamente al conjunto de datos iniciales (*Seed*).

---

## Credenciales de Prueba

Todas las cuentas tienen la contraseña: **`Sitec.2026`**

| Email | Organización | Rol |
| --- | --- | --- |
| `admin@norte.test` | Cooperativa Norte | Admin |
| `agente1@norte.test` | Cooperativa Norte | Agente |
| `agente2@norte.test` | Cooperativa Norte | Agente |
| `user1@norte.test` | Cooperativa Norte | Solicitante |
| `user2@norte.test` | Cooperativa Norte | Solicitante |
| `admin@sur.test` | Bufete Sur | Admin |
| `agente1@sur.test` | Bufete Sur | Agente |
| `user1@sur.test` | Bufete Sur | Solicitante |

---

## Autenticación en Swagger

1. Ejecuta `POST /api/v1/auth/login` con las credenciales elegidas.
2. Copia el `accessToken` de la respuesta.
3. Clic en **Authorize 🔓** en la parte superior derecha.
4. Escribe `Bearer <tu_token>` y clic en **Authorize**.

---

## Variables de Entorno

Copia `.env.example` a `.env` y ajusta si es necesario:

```bash
JWT_SECRET=SuperSecretKeyForMesaSitec2026_AtLeast32BytesLong!
SEED_FECHA_BASE=2026-01-15T08:00:00Z
VITE_API_URL=http://localhost:5080/api/v1
```

---

## Ejecutar Pruebas Unitarias

```bash
cd backend && dotnet test
```

Mínimo 9 pruebas en verde cubriendo RN-02 (máquina de estados), RN-04 (cálculo SLA) y RN-06 (motivos).

---

## Estructura del Proyecto

```
/
├── README.md
├── DECISIONES.md
├── .env.example
├── backend/
│   ├── Dockerfile                   # Imagen Docker multi-stage para .NET 8
│   ├── src/
│   │   ├── MesaSitec.Api/           # Controladores, Middlewares, Swagger, CORS
│   │   ├── MesaSitec.Aplicacion/    # DTOs, Interfaces, Servicios
│   │   ├── MesaSitec.Dominio/       # Entidades, Enums, Excepciones
│   │   └── MesaSitec.Infraestructura/ # DbContext, Migraciones, Seeder
│   └── tests/
│       └── MesaSitec.Tests/         # Pruebas unitarias xUnit
└── frontend/
    ├── vercel.json                  # Reglas de enrutamiento SPA para Vercel
    ├── public/
    │   └── favicon.svg              # Isotipo oficial Escudo Multi-Tenant
    └── src/
        ├── api/        # httpClient centralizado con interceptores
        ├── components/ # Componentes reutilizables (Navbar, PanelTransiciones, etc.)
        ├── views/      # Vistas (Login, Listado, Detalle, Nueva, Editar)
        ├── stores/     # Pinia (authStore)
        ├── types/      # DTOs tipados en TypeScript
        └── router/     # Vue Router con guard de autenticación
```

---

## Qué está implementado

### Backend
- ✅ Clean Architecture (Api / Aplicacion / Dominio / Infraestructura)
- ✅ SQLite con migraciones automáticas al arrancar
- ✅ Contenerización con Docker (`backend/Dockerfile`) para despliegue continuo en la nube
- ✅ JWT HS256 con claims `sub`, `tenantId`, `rol`, `email`, expiración 8h
- ✅ BCrypt para contraseñas
- ✅ Aislamiento multi-tenant estricto (RN-01) — recursos de otro tenant devuelven 404
- ✅ Máquina de estados completa (RN-02)
- ✅ Permisos por rol (RN-03)
- ✅ Cálculo y recálculo de SLA (RN-04)
- ✅ Validación de agente al asignar (RN-05)
- ✅ Motivo obligatorio en resolver/cancelar (RN-06)
- ✅ Código correlativo por tenant y año (RN-07)
- ✅ Los 9 endpoints del contrato bajo `/api/v1`
- ✅ Paginación, filtros y ordenamiento server-side en `GET /solicitudes`
- ✅ Respuestas de error en `application/problem+json` con campo `codigo`
- ✅ `GET /health` sin autenticación devuelve `{ "estado": "ok" }`
- ✅ Swagger con esquema Bearer en `/swagger`
- ✅ Política CORS global configurada para permitir integración con Vercel
- ✅ Middleware global de excepciones (sin stack traces al cliente)
- ✅ Datos semilla con `SEED_FECHA_BASE` (25 solicitudes Norte, 8 Sur)
- ✅ 9 pruebas unitarias con xUnit

### Frontend
- ✅ Vue 3 + TypeScript strict + Vite + Pinia + Vue Router
- ✅ Notificaciones Toast universales (`vue3-toastify`) para inicio/cierre de sesión, creación, edición y transiciones
- ✅ Tema Cyberpunk Violeta Nocturno (`#7c3aed`) con fondo Obsidiana
- ✅ Isotipo vectorial SVG personalizado de Escudo Multi-Tenant (`favicon.svg`)
- ✅ Experiencia de asignación diferenciada por rol:
  - Botón "Asignarme Ticket" directo para Agentes en solicitudes `Nueva`.
  - Botón dinámico "Asignar" / "Reasignar Ticket" para Admins.
  - Protección de "Iniciar Trabajo" únicamente al agente asignado.
  - Protección de "Ver Detalle" deshabilitado con tooltip `Este ticket esta asigando a otro agente` en el listado para agentes no responsables.
- ✅ Guard de rutas privadas
- ✅ Módulo HTTP centralizado con inyección de token y redirección en 401
- ✅ DTOs tipados alineados con el contrato de la API
- ✅ Vista `/login` con manejo de errores y selección rápida de perfiles
- ✅ Vista `/solicitudes` con filtros, búsqueda y paginación server-side
- ✅ Vista `/solicitudes/nueva` (formulario dedicado)
- ✅ Vista `/solicitudes/:id` con botones de acción según estado y rol
- ✅ Vista `/solicitudes/:id/editar` (mismo componente en modo edición)
- ✅ Botones de acción no permitidos no se renderizan en el DOM
- ✅ Todos los `data-testid` requeridos presentes
- ✅ `paginacion-info` con formato exacto `Página X de Y — Z resultados`
- ✅ Configuración `vercel.json` para evitar errores 404 en refrescos SPA
