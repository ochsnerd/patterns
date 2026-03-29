namespace TypedId;

using System.Text.Json;
using System.Text.Json.Serialization;

public class IdJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Id<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var entityType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(IdJsonConverter<>).MakeGenericType(entityType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}
