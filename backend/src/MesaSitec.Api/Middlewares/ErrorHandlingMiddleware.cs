using System.Text.Json;
using MesaSitec.Dominio.Excepciones;

namespace MesaSitec.Api.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ReglaNegocioException ex)
        {
            _logger.LogWarning(ex, "Regla de negocio violada: {Codigo}", ex.Codigo);
            await EscribirErrorAsync(context, ex.StatusCode, ex.Codigo, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Recurso no encontrado: {Message}", ex.Message);
            await EscribirErrorAsync(context, 404, "RECURSO_NO_ENCONTRADO", ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Acceso no autorizado: {Message}", ex.Message);
            await EscribirErrorAsync(context, 401, "NO_AUTENTICADO", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en el servidor");
            await EscribirErrorAsync(context, 500, "ERROR_INTERNO", "Ocurrió un error inesperado en el servidor.");
        }
    }

    private static Task EscribirErrorAsync(HttpContext context, int statusCode, string codigo, string detail)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var payload = ProblemPayloadFactory.Crear(statusCode, codigo, detail);
        var json = JsonSerializer.Serialize(payload, ProblemPayloadFactory.JsonOptions);

        return context.Response.WriteAsync(json);
    }
}
