namespace TypedId;

using System.Text.Json;
using System.Text.Json.Serialization;

public class IdJsonConverter<TEntity> : JsonConverter<Id<TEntity>>
    where TEntity : IIdentifiable<TEntity>
{
    public override Id<TEntity> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        reader.Read(); // PropertyName "Value"
        reader.Read(); // The actual value
        var value = reader.GetGuid();
        reader.Read(); // EndObject

        return new Id<TEntity>(value);
    }

    public override void Write(Utf8JsonWriter writer, Id<TEntity> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Value", value.Value);
        writer.WriteEndObject();
    }

    public override Id<TEntity> ReadAsPropertyName(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetString();
        return Id<TEntity>.TryParse(value, out var result)
            ? result
            : throw new JsonException($"Invalid Id<{typeof(TEntity).Name}> property name: {value}");
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, Id<TEntity> value, JsonSerializerOptions options) =>
        writer.WritePropertyName(value.ToString());
}
