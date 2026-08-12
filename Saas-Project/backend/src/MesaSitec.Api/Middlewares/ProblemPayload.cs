using System.Text.Json;
using System.Text.Json.Serialization;

namespace MesaSitec.Api.Middlewares;

public class ProblemPayload
{
    public required string Type { get; init; }
    public required string Title { get; init; }
    public required int Status { get; init; }
    public required string Detail { get; init; }
    public required string Codigo { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Errores { get; init; }
}

public static class ProblemPayloadFactory
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static ProblemPayload Crear(int statusCode, string codigo, string detail, IDictionary<string, string[]>? errores = null)
    {
        var titulo = codigo switch
        {
            "NO_AUTENTICADO"         => "No autenticado",
            "OPERACION_NO_PERMITIDA" => "Operación no permitida",
            "RECURSO_NO_ENCONTRADO"  => "Recurso no encontrado",
            "TRANSICION_INVALIDA"    => "Transición inválida",
            "AGENTE_INVALIDO"        => "Agente inválido",
            "MOTIVO_REQUERIDO"       => "Motivo requerido",
            "PARAMETRO_INVALIDO"     => "Parámetro inválido",
            "VALIDACION"             => "Error de validación",
            _                        => "Error interno"
        };

        var slug = codigo.ToLower().Replace('_', '-');

        return new ProblemPayload
        {
            Type = $"https://mesasitec.local/errores/{slug}",
            Title = titulo,
            Status = statusCode,
            Detail = detail,
            Codigo = codigo,
            Errores = errores
        };
    }
}
