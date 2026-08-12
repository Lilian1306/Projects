# 📐 DECISIONES.md - Decisiones de Diseño, Arquitectura e Incidencias

Este documento consolida las decisiones de arquitectura, decisiones técnicas con alternativas descartadas, la bitácora de incidencias resueltas, la declaración de uso de IA y las mejoras futuras del proyecto **MesaSitec**.

---

## 1. 🏛️ Decisiones de Arquitectura y Diseño

### 1.1. Arquitectura Limpia (*Clean Architecture*)
Se organizó el backend en cuatro proyectos claramente desacoplados:
- **`MesaSitec.Dominio`**: Entidades del modelo de negocio (`Solicitud`, `Usuario`, `Categoria`, `Tenant`), Enums (`Rol`, `Estado`, `Prioridad`) y excepciones del dominio (`ReglaNegocioException`). Libre de dependencias externas de infraestructura o frameworks.
- **`MesaSitec.Infraestructura`**: Persistencia mediante Entity Framework Core (`MesaSitecDbContext`) y la clase de datos semilla (`MesaSitecDbContextSeed`).
- **`MesaSitec.Aplicacion`**: Lógica de aplicación, interfaces (`IAuthService`, `ISolicitudService`, `ICategoriaService`, `IJwtTokenGenerator`), DTOs y servicios de negocio.
- **`MesaSitec.Api`**: Capa de presentación REST basada en ASP.NET Core Web API con controladores, middleware global de excepciones y Swagger.

### 1.2. Tres Decisiones Técnicas con Alternativas Descartadas

1. **Arquitectura Limpia en 4 Capas (Api / Aplicación / Dominio / Infraestructura)**:
   - **Alternativa descartada:** Arquitectura monolítica de controlador-directo-a-base-de-datos (*Transaction Script*).
   - **Por qué se descartó:** Mezclaba la lógica de negocio con la persistencia. La Clean Architecture garantizó que las reglas de negocio (RN-01 a RN-07) queden totalmente aisladas en el proyecto `Dominio` sin dependencias de frameworks externos.

2. **Convertidor Flexible de Identificadores (`FlexibleGuidJsonConverter`)**:
   - **Alternativa descartada:** Exigir únicamente cadenas GUID completas de 36 caracteres en todas las peticiones JSON.
   - **Por qué se descartó:** Dificultaba las pruebas manuales en Swagger y clientes API. El convertidor permite ingresar identificadores cortos o números (`"1"`, `1`, `"2"`) mapeándolos automáticamente a su equivalente `Guid` en el backend sin romper el contrato de datos.

3. **UX Diferenciada y Protección por Rol en Asignación de Solicitudes**:
   - **Alternativa descartada:** Mostrar el mismo modal con selector de GUIDs de otros usuarios a todos los roles indiscriminadamente.
   - **Por qué se descartó:** Era informal e innecesariamente técnico para un Agente. Se implementó auto-asignación directa ("Asignarme Ticket") en estado `Nueva`, deshabilitación de "Ver Detalle" en el listado para tickets de compañeros con el tooltip `🔒 Asignado a otro agente`, y reservó el modal completo con el botón "Reasignar Ticket" únicamente para Administradores.

### 1.3. Aislamiento Multi-Tenant Estricto
- Cada petición autenticada extrae el `TenantId` del Claim del token JWT.
- Las consultas y mutaciones en base de datos están estrictamente filtradas por `TenantId`, garantizando que ninguna organización pueda ver o modificar datos de otra (*Data Isolation*).

### 1.4. Reglas de Negocio y SLA
- Las solicitudes calculan automáticamente su fecha límite de SLA al crearse basándose en el `SlaHoras` de la categoría seleccionada y la prioridad.
- Las transiciones de estado (*Nueva ➔ Asignada ➔ En Proceso ➔ Resuelta ➔ Cerrada*) imponen validaciones estrictas y restricciones según el rol del usuario (Admin, Agente, Solicitante).

### 1.5. Notificaciones Toast Universales (`vue3-toastify`) y Tema Cyberpunk
- Se integró `vue3-toastify` configurado en `main.ts` con tema oscuro (`theme: 'dark'`) para notificar al usuario sobre inicios/cierres de sesión, creación/edición de solicitudes y transiciones.
- Se actualizó el diseño visual en Tailwind CSS a una paleta **Cyberpunk Violeta Nocturno (`#7c3aed`)** con fondos de tarjeta en **Obsidiana (`#090714` / `#130f26`)** e isotipo SVG personalizado (`favicon.svg`).

### 1.6. Contenerización Multi-Stage con Docker y Comportamiento de Persistencia
- Se creó `backend/Dockerfile` utilizando compilación en dos etapas (*build* y *runtime*) sobre .NET 8 oficial de Microsoft, permitiendo el despliegue automático en servicios de contenedores como Render.
- **Persistencia en Producción**: Al usar **SQLite** local dentro del contenedor sobre el plan gratuito de Render, el sistema utiliza almacenamiento efímero (temporal). Tras 15 minutos de inactividad, Render suspende la aplicación (*sleep*); al reiniciar o despertar, los datos creados en sesión se restauran automáticamente al conjunto inicial de datos semilla (*SeedData*).

### 1.7. Estrategia de Ramificación en Git (*Git Flow*) y Metodología de Trabajo
- **Estructura de Ramas Principales**:
  - `main`: Rama de producción para despliegues estables.
  - `development`: Rama de integración continua y base para el desarrollo.
- **Flujo de Trabajo por Funcionalidades**:
  - Todas las ramas auxiliares de características **nacen a partir de la rama `development`**.
  - Al terminar los cambios, se publica en GitHub, se crea el **Pull Request (PR)** con destino a `development` y se realiza la integración (*merge*).
  - **Regla de Cierre de Ramas**: Una vez completado el *merge* de un PR en GitHub, esa rama queda consolidada y cerrada; para nuevos cambios se crea una rama nueva a partir de `development` actualizada.

---

## 2. 📋 Bitácora de Incidencias y Soluciones (Puntos de Bloqueo)

### 💥 El Punto de Mayor Bloqueo y su Resolución
* **El Obstáculo Principal:** El bloqueo de autenticación por políticas **CORS** y el error `404: NOT_FOUND` al recargar rutas en Vercel tras conectar el Frontend (Vercel) con el Backend en la nube (Render).
* **Cómo se resolvió:** 
  1. Se identificó en la consola del navegador (F12) que Vercel estaba siendo rechazado por la política estricta de CORS en `Program.cs`, actualizándola a `AllowAnyOrigin()`.
  2. Se creó la regla de *rewrites* hacia `index.html` en `frontend/vercel.json` para permitir que `vue-router` maneje la navegación SPA sin errores de servidor.

---

### Registro Completo de Incidencias:

### Incidencia 1: Error de Sintaxis XML en la Solución (`MesaSitec.slnx`)
* **Problema**: `MSB4025: The 'Folder' start tag on line 2 position 4 does not match the end tag of 'Solution'`.
* **Causa**: Faltaba la etiqueta de cierre `</Folder>` en la sección `/src/`.
* **Solución**: Se corrigió la estructura XML en `MesaSitec.slnx` añadiendo las etiquetas de cierre correspondientes.

### Incidencia 2: Incompatibilidad de versión del SDK .NET en entorno local
* **Problema**: `You must install or update .NET to run this application. Framework 'Microsoft.AspNetCore.App', version '8.0.0' not found.`
* **Causa**: La máquina host tenía instalado .NET 10 (x64) y el proyecto estaba fijado a .NET 8 sin regla de avance.
* **Solución**: Se agregó la propiedad `<RollForward>LatestMajor</RollForward>` a los archivos `.csproj` y se creó `global.json`.

### Incidencia 3: Excepción de Formato en Datos Semilla (`Guid.Parse`)
* **Problema**: `Unhandled exception. System.FormatException: Guid string should only contain hexadecimal characters`.
* **Causa**: Los identificadores de usuarios en la semilla contenían el carácter `'u'`.
* **Solución**: Se sustituyó el carácter `'u'` por el carácter hexadecimal `'b'` en `MesaSitecDbContextSeed.cs`.

### Incidencia 4: Error `401 Unauthorized` al Autenticar en Swagger
* **Problema**: Respuestas `401 Unauthorized: invalid_token` desde Swagger.
* **Causa**: Swagger anteponía automáticamente `"Bearer "`.
* **Solución**: Se actualizó `SecurityScheme` en `Program.cs` a `SecuritySchemeType.ApiKey`.

### Incidencia 5: Error `400 Bad Request` al Crear Solicitudes en Swagger
* **Problema**: `The JSON value could not be converted to System.Guid. Path: $.categoriaId`.
* **Causa**: Se enviaban números cortos (`"3"`) en lugar del GUID completo.
* **Solución**: Se desarrolló e implementó el convertidor personalizado `FlexibleGuidJsonConverter`.

### Incidencia 6: Error `404: NOT_FOUND` al Recargar Páginas en Vercel
* **Problema**: Al recargar la página en rutas como `/solicitudes` o `/login` en Vercel, se mostraba una pantalla `404: NOT_FOUND`.
* **Causa**: Vercel no reconocía el enrutamiento client-side de `vue-router` en aplicaciones de una sola página (SPA).
* **Solución**: Se creó el archivo `frontend/vercel.json` con la regla de *rewrites* hacia `/index.html`.

### Incidencia 7: Bloqueo de Conexión CORS en el Despliegue en la Nube (Vercel ➔ Render)
* **Problema**: `Error al iniciar sesión. Verifique sus credenciales.` al autenticarse desde Vercel.
* **Causa**: La política CORS en `Program.cs` solo permitía el origen `http://localhost:5173`.
* **Solución**: Se actualizó la política CORS en `Program.cs` a `AllowAnyOrigin()` para aceptar peticiones desde dominios Vercel.

### Incidencia 8: Error `dotnet: command not found` al Desplegar Backend en Render
* **Problema**: Fallo en la fase de construcción en Render.
* **Causa**: Render seleccionaba por defecto el entorno ejecutable de Node.js.
* **Solución**: Se agregó `backend/Dockerfile` y se configuró el entorno a **Docker** en Render.

### Incidencia 9: Conflictos de Historial y Errores al Intentar `git pull` / `git push` sobre Ramas Ya Fusionadas (*Merged*)
* **Problema**: Errores de sincronización (`rejected (non-fast-forward)`) al intentar enviar nuevos commits a una rama auxiliar cuyo PR ya había sido fusionado previamente en GitHub.
* **Causa**: Al realizar el *merge* de un PR en GitHub, la rama destino (`development`) avanza con un nuevo commit de integración, generando divergencias si se reutiliza la rama cerrada.
* **Solución**: Sincronizar `development` localmente y crear siempre una rama fresca derivada de `development` para nuevos cambios.

---

## 3. 🤖 Declaración de Uso de Inteligencia Artificial (IA) y Autoría

En cumplimiento de las buenas prácticas de transparencia técnica y desarrollo colaborativo en pareja (*Pair Programming*), se especifica el desglose de aportes entre el Desarrollador y el Asistente de IA:

### 3.1. Rol y Aportes del Desarrollador (Humano - 48%)
- **Definición de Requisitos y Arquitectura**: Diseño del modelo de dominio, reglas de negocio (RN-01 a RN-07), aislamiento Multi-Tenant y estructura Clean Architecture.
- **Criterio Técnico y Decisiones de UI/UX**: Selección y refactorización de la experiencia por rol (auto-asignación para Agentes vs. modal con GUIDs para Admins, restricciones de inicio de trabajo, botón "Reasignar Ticket" y tooltip de bloqueo), elección de la paleta Cyberpunk Violeta y definición del flujo de trabajo Git Flow.
- **Validación y Despliegue en Producción**: Configuración y pruebas continuas de autenticación, integración en la nube (Vercel + Render), verificación de CORS y pruebas de regresión.

### 3.2. Rol y Aportes del Asistente de IA (Antigravity Assistant - 52%)
- **Generación de Código Base y Boilerplate**: Asistencia en la aceleración de controladores, servicios de aplicación, DTOs, entidades de Entity Framework Core y clases Tailwind CSS.
- **Diagnóstico y Resolución de Incidencias**: Análisis y solución de bloqueos de archivos en re-compilaciones, configuración de `FlexibleGuidJsonConverter`, reglas de *rewrites* en `vercel.json` y empaquetado multi-stage con `Dockerfile`.
- **Documentación Técnica**: Consolidación y estructuración de los documentos `README.md` y `DECISIONES.md`.

### 3.3. Porcentaje Estimado de Distribución del Proyecto

| Componente | Desarrollador (Humano) | Asistente de IA |
| --- | :---: | :---: |
| **Arquitectura, Lógica de Negocio y Reglas RN-01..07** | **60%** | 40% |
| **Generación de Código Base y Boilerplate** | **40%** | **60%** |
| **Diseño UI/UX, Componentes Vue 3 y Estilos Tailwind** | **45%** | 55% |
| **Configuración de Despliegue (Docker, Vercel, Render)** | 40% | **60%** |
| **Resolución de Incidencias y Debugging** | **50%** | 50% |
| **Documentación Técnica (README & DECISIONES)** | **50%** | **50%** |
| **TOTAL ESTIMADO GLOBAL DEL PROYECTO** | **48%** | **52%** |

---

## 4. 🔮 ¿Qué se haría distinto con una semana más de tiempo?

1. **Notificaciones Push en Tiempo Real (SignalR / WebSockets)**:
   - Implementar un hub de SignalR para notificar instantáneamente a los agentes cuando un usuario crea una nueva solicitud o cuando se le asigna un ticket sin necesidad de recargar la página.
2. **Pruebas End-to-End (E2E) con Playwright / Cypress**:
   - Automatizar flujos completos de pruebas de usuario en el navegador (Login ➔ Crear Solicitud ➔ Asignar ➔ Resolver ➔ Cerrar) integrados en GitHub Actions.
