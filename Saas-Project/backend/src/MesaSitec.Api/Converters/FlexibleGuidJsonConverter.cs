using System.Text.Json;
using System.Text.Json.Serialization;

namespace MesaSitec.Api.Converters;

public class FlexibleGuidJsonConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (string.IsNullOrWhiteSpace(stringValue)) return Guid.Empty;

            if (Guid.TryParse(stringValue, out var guid))
            {
                return guid;
            }

            // Permite números cortos pasados como string ("1", "2", "3")
            if (long.TryParse(stringValue, out var longValue))
            {
                return ConvertNumberToGuid(longValue);
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt64(out var longValue))
            {
                return ConvertNumberToGuid(longValue);
            }
        }

        throw new JsonException($"No se pudo convertir '{reader.GetString()}' a un Guid válido.");
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    private static Guid ConvertNumberToGuid(long number)
    {
        // Formatea el número a un GUID válido de 32 dígitos con guiones: 00000000-0000-0000-0000-00000000000X
        string hex = number.ToString("D32");
        string formatted = $"{hex[..8]}-{hex.Substring(8, 4)}-{hex.Substring(12, 4)}-{hex.Substring(16, 4)}-{hex[20..]}";
        return Guid.Parse(formatted);
    }
}
