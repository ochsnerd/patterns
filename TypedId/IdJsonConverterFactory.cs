namespace TypedId;

using System.Text.Json;
using System.Text.Json.Serialization;

public class IdJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.BaseType is { IsGenericType: true } baseType
        && baseType.GetGenericTypeDefinition() == typeof(Id<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(IdJsonConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}
