using System.Text;
using System.Text.Json;
using MesaSitec.Api.Middlewares;
using MesaSitec.Aplicacion.Interfaces;
using MesaSitec.Aplicacion.Servicios;
using MesaSitec.Infraestructura.Persistencia;
using MesaSitec.Infraestructura.Persistencia.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new MesaSitec.Api.Converters.FlexibleGuidJsonConverter());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // Reemplaza el ValidationProblemDetails nativo de ASP.NET Core por el formato
        // problem+json obligatorio del contrato (con "codigo" y "errores").
        options.InvalidModelStateResponseFactory = context =>
        {
            var errores = context.ModelState
                .Where(kvp => kvp.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => char.ToLowerInvariant(kvp.Key[0]) + kvp.Key[1..],
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

            var payload = ProblemPayloadFactory.Crear(
                StatusCodes.Status422UnprocessableEntity,
                "VALIDACION",
                "Uno o más campos no son válidos.",
                errores);

            return new ObjectResult(payload)
            {
                StatusCode = StatusCodes.Status422UnprocessableEntity,
                ContentTypes = { "application/problem+json" }
            };
        };
    });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MesaSitec API",
        Version = "v1",
        Description = "API REST multi-tenant para el sistema MesaSitec de gestión de solicitudes."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer ' seguido de su token JWT. Ejemplo: 'Bearer eyJhbGciOi...'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var dbPath = Path.Combine(AppContext.BaseDirectory, "mesasitec.db");
builder.Services.AddDbContext<MesaSitecDbContext>(options =>
{
    options.UseSqlite($"Data Source={dbPath}");
});
builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<MesaSitecDbContext>());

builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<ISolicitudService, SolicitudService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? builder.Configuration["Jwt:Secret"]
    ?? "SuperSecretKeyForMesaSitec2026_AtLeast32BytesLong!";
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
    // Sin esto, un token ausente/inválido/expirado responde 401 con body vacío,
    // incumpliendo el contrato (que exige codigo:NO_AUTENTICADO en problem+json).
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var payload = ProblemPayloadFactory.Crear(
                StatusCodes.Status401Unauthorized,
                "NO_AUTENTICADO",
                "El token de acceso es inválido, expiró o no fue proporcionado.");

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload, ProblemPayloadFactory.JsonOptions));
        },
        OnForbidden = async context =>
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            var payload = ProblemPayloadFactory.Crear(
                StatusCodes.Status403Forbidden,
                "OPERACION_NO_PERMITIDA",
                "No tiene permisos para acceder a este recurso.");

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload, ProblemPayloadFactory.JsonOptions));
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MesaSitecDbContext>();
    await context.Database.MigrateAsync();
    await MesaSitecDbContextSeed.SeedAsync(context);
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MesaSitec API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { estado = "ok" }));
app.MapControllers();

app.Run();
