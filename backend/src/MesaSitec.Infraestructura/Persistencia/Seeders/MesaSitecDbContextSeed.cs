using MesaSitec.Dominio.Entidades;
using MesaSitec.Dominio.Enums;
using Microsoft.EntityFrameworkCore;

namespace MesaSitec.Infraestructura.Persistencia.Seeders;

public static class MesaSitecDbContextSeed
{
    public static async Task SeedAsync(MesaSitecDbContext context)
    {
        // Si ya existen datos en Tenants, la siembra ya fue realizada
        if (await context.Tenants.AnyAsync())
        {
            return;
        }

        // 1. Obtener la fecha base desde variable de entorno o usar el valor por defecto
        var envFechaBase = Environment.GetEnvironmentVariable("SEED_FECHA_BASE");
        DateTime fechaBase = DateTime.TryParse(envFechaBase, out var parsedDate) 
            ? parsedDate.ToUniversalTime() 
            : new DateTime(2026, 1, 15, 8, 0, 0, DateTimeKind.Utc);

        string passwordHash = BCrypt.Net.BCrypt.HashPassword("Sitec.2026");

        // 2. Crear Organizaciones (Tenants)
        var tenantNorte = new Tenant
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Nombre = "Cooperativa Norte",
            Activo = true
        };

        var tenantSur = new Tenant
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Nombre = "Bufete Sur",
            Activo = true
        };

        context.Tenants.AddRange(tenantNorte, tenantSur);

        // 3. Crear Usuarios Semilla
        var adminNorte = new Usuario { Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"), TenantId = tenantNorte.Id, Email = "admin@norte.test", PasswordHash = passwordHash, Nombre = "Admin Norte", Rol = Rol.Admin, Activo = true };
        var agente1Norte = new Usuario { Id = Guid.Parse("a2222222-1111-1111-1111-111111111111"), TenantId = tenantNorte.Id, Email = "agente1@norte.test", PasswordHash = passwordHash, Nombre = "Agente Uno Norte", Rol = Rol.Agente, Activo = true };
        var agente2Norte = new Usuario { Id = Guid.Parse("a3333333-1111-1111-1111-111111111111"), TenantId = tenantNorte.Id, Email = "agente2@norte.test", PasswordHash = passwordHash, Nombre = "Agente Dos Norte", Rol = Rol.Agente, Activo = true };
        var user1Norte = new Usuario { Id = Guid.Parse("b1111111-1111-1111-1111-111111111111"), TenantId = tenantNorte.Id, Email = "user1@norte.test", PasswordHash = passwordHash, Nombre = "Usuario Uno Norte", Rol = Rol.Solicitante, Activo = true };
        var user2Norte = new Usuario { Id = Guid.Parse("b2222222-1111-1111-1111-111111111111"), TenantId = tenantNorte.Id, Email = "user2@norte.test", PasswordHash = passwordHash, Nombre = "Usuario Dos Norte", Rol = Rol.Solicitante, Activo = true };

        var adminSur = new Usuario { Id = Guid.Parse("a1111111-2222-2222-2222-222222222222"), TenantId = tenantSur.Id, Email = "admin@sur.test", PasswordHash = passwordHash, Nombre = "Admin Sur", Rol = Rol.Admin, Activo = true };
        var agente1Sur = new Usuario { Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"), TenantId = tenantSur.Id, Email = "agente1@sur.test", PasswordHash = passwordHash, Nombre = "Agente Uno Sur", Rol = Rol.Agente, Activo = true };
        var user1Sur = new Usuario { Id = Guid.Parse("b1111111-2222-2222-2222-222222222222"), TenantId = tenantSur.Id, Email = "user1@sur.test", PasswordHash = passwordHash, Nombre = "Usuario Uno Sur", Rol = Rol.Solicitante, Activo = true };

        context.Usuarios.AddRange(adminNorte, agente1Norte, agente2Norte, user1Norte, user2Norte, adminSur, agente1Sur, user1Sur);

        // 4. Crear Categorías Semilla (IDs 1 a 4 para Norte, 5 a 8 para Sur)
        var catIncidenteNorte = new Categoria { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), TenantId = tenantNorte.Id, Nombre = "Incidente", SlaHoras = 8, Activo = true };
        var catRequerimientoNorte = new Categoria { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), TenantId = tenantNorte.Id, Nombre = "Requerimiento", SlaHoras = 40, Activo = true };
        var catConsultaNorte = new Categoria { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), TenantId = tenantNorte.Id, Nombre = "Consulta", SlaHoras = 24, Activo = true };
        var catFallaNorte = new Categoria { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), TenantId = tenantNorte.Id, Nombre = "Falla crítica", SlaHoras = 4, Activo = true };

        var catIncidenteSur = new Categoria { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), TenantId = tenantSur.Id, Nombre = "Incidente", SlaHoras = 8, Activo = true };
        var catRequerimientoSur = new Categoria { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), TenantId = tenantSur.Id, Nombre = "Requerimiento", SlaHoras = 40, Activo = true };
        var catConsultaSur = new Categoria { Id = Guid.Parse("00000000-0000-0000-0000-000000000007"), TenantId = tenantSur.Id, Nombre = "Consulta", SlaHoras = 24, Activo = true };
        var catFallaSur = new Categoria { Id = Guid.Parse("00000000-0000-0000-0000-000000000008"), TenantId = tenantSur.Id, Nombre = "Falla crítica", SlaHoras = 4, Activo = true };

        context.Categorias.AddRange(
            catIncidenteNorte, catRequerimientoNorte, catConsultaNorte, catFallaNorte,
            catIncidenteSur, catRequerimientoSur, catConsultaSur, catFallaSur);

        // 5. Crear Solicitudes (25 para Cooperativa Norte, 8 para Bufete Sur)
        var solicitudes = new List<Solicitud>();

        var catsNorte = new[] { catIncidenteNorte, catRequerimientoNorte, catConsultaNorte, catFallaNorte };
        var usersNorte = new[] { user1Norte, user2Norte };
        var agentesNorte = new[] { agente1Norte, agente2Norte };
        var estados = new[] { Estado.Nueva, Estado.Asignada, Estado.EnProceso, Estado.Resuelta, Estado.Cerrada, Estado.Cancelada };
        var prioridades = new[] { Prioridad.Baja, Prioridad.Media, Prioridad.Alta, Prioridad.Critica };

        // --- 25 Solicitudes Cooperativa Norte ---
        for (int i = 1; i <= 25; i++)
        {
            var cat = catsNorte[(i - 1) % catsNorte.Length];
            var solicitante = usersNorte[(i - 1) % usersNorte.Length];
            var prioridad = prioridades[(i - 1) % prioridades.Length];
            var estado = estados[(i - 1) % estados.Length];

            // Para garantizar al menos 5 vencidas en Norte:
            // Si i <= 5, creamos con fecha del pasado de modo que SLA expire antes de fechaBase
            DateTime fechaCreacion = i <= 5 
                ? fechaBase.AddHours(-100) 
                : fechaBase.AddHours(- (i * 2));

            // Garantizar al menos 3 resueltas (i = 4, 10, 16)
            if (i == 4 || i == 10 || i == 16)
            {
                estado = Estado.Resuelta;
            }

            var sol = new Solicitud
            {
                Id = Guid.NewGuid(),
                TenantId = tenantNorte.Id,
                Codigo = $"SOL-2026-{i:D5}",
                Titulo = $"Solicitud de soporte #{i} en Cooperativa Norte",
                Descripcion = $"Descripción detallada para la solicitud de prueba número {i} en Cooperativa Norte.",
                CategoriaId = cat.Id,
                Prioridad = prioridad,
                Estado = Estado.Nueva,
                SolicitanteId = solicitante.Id,
                FechaCreacion = fechaCreacion
            };

            sol.CalcularFechaLimiteSla(cat.SlaHoras);

            // Ajustar estado y propiedades adicionales según el tipo
            if (estado == Estado.Asignada || estado == Estado.EnProceso)
            {
                var agente = agentesNorte[(i - 1) % agentesNorte.Length];
                sol.Asignar(agente.Id);
                if (estado == Estado.EnProceso)
                {
                    sol.Iniciar();
                }
            }
            else if (estado == Estado.Resuelta)
            {
                var agente = agentesNorte[(i - 1) % agentesNorte.Length];
                sol.Asignar(agente.Id);
                sol.Iniciar();
                sol.Resolver("Se solucionó el problema restableciendo las credenciales del usuario y probando el acceso.", fechaCreacion.AddHours(2));
            }
            else if (estado == Estado.Cerrada)
            {
                var agente = agentesNorte[(i - 1) % agentesNorte.Length];
                sol.Asignar(agente.Id);
                sol.Iniciar();
                sol.Resolver("Problema resuelto satisfactoriamente por el equipo de soporte técnico.", fechaCreacion.AddHours(2));
                sol.Cerrar();
            }
            else if (estado == Estado.Cancelada)
            {
                sol.Cancelar("Solicitud duplicada creada por error por el usuario solicitante.");
            }

            solicitudes.Add(sol);
        }

        // --- 8 Solicitudes Bufete Sur ---
        for (int i = 1; i <= 8; i++)
        {
            var cat = new[] { catIncidenteSur, catRequerimientoSur, catConsultaSur, catFallaSur }[(i - 1) % 4];
            var prioridad = prioridades[(i - 1) % prioridades.Length];
            var estado = estados[(i - 1) % estados.Length];

            DateTime fechaCreacion = fechaBase.AddHours(-(i * 3));

            var sol = new Solicitud
            {
                Id = Guid.NewGuid(),
                TenantId = tenantSur.Id,
                Codigo = $"SOL-2026-{i:D5}",
                Titulo = $"Solicitud en Bufete Sur #{i}",
                Descripcion = $"Detalle de la solicitud de prueba número {i} para la organización Bufete Sur.",
                CategoriaId = cat.Id,
                Prioridad = prioridad,
                Estado = Estado.Nueva,
                SolicitanteId = user1Sur.Id,
                FechaCreacion = fechaCreacion
            };

            sol.CalcularFechaLimiteSla(cat.SlaHoras);

            if (estado == Estado.Asignada || estado == Estado.EnProceso)
            {
                sol.Asignar(agente1Sur.Id);
                if (estado == Estado.EnProceso) sol.Iniciar();
            }
            else if (estado == Estado.Resuelta)
            {
                sol.Asignar(agente1Sur.Id);
                sol.Iniciar();
                sol.Resolver("Problema resuelto satisfactoriamente por el equipo de soporte.", fechaCreacion.AddHours(2));
            }
            else if (estado == Estado.Cerrada)
            {
                sol.Asignar(agente1Sur.Id);
                sol.Iniciar();
                sol.Resolver("Problema resuelto satisfactoriamente por el equipo de soporte.", fechaCreacion.AddHours(2));
                sol.Cerrar();
            }
            else if (estado == Estado.Cancelada)
            {
                sol.Cancelar("Cancelada debido a ticket repetido en el sistema.");
            }

            solicitudes.Add(sol);
        }

        context.Solicitudes.AddRange(solicitudes);

        await context.SaveChangesAsync();
    }
}
