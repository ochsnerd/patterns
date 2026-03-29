namespace TypedId;

using System.Text.Json;

public static class Json
{
    public static JsonSerializerOptions Options { get; } =
        new(JsonSerializerOptions.Web) { Converters = { new IdJsonConverterFactory() } };
}
