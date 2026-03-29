namespace TypedId;

using System.Text.Json;
using System.Text.Json.Serialization;

public class IdJsonConverter<T> : JsonConverter<T>
    where T : Id<T>, IIdFactory<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        reader.Read(); // PropertyName "Value"
        reader.Read(); // The actual value
        var value = reader.GetGuid();
        reader.Read(); // EndObject

        return T.Create(value);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Value", value.Value);
        writer.WriteEndObject();
    }

    public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return Id<T>.TryParse(value, out var result)
            ? result
            : throw new JsonException($"Invalid {typeof(T).Name} property name: {value}");
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
        writer.WritePropertyName(value.ToString());
}
