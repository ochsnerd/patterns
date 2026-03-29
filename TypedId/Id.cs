#pragma warning disable CA1000
namespace TypedId;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

[JsonConverter(typeof(IdJsonConverterFactory))]
public record Id<T>(Guid Value) : IParsable<T>
    where T : Id<T>, IIdFactory<T>
{
    public Guid Value { get; } =
        Value != Guid.Empty ? Value : throw new ArgumentException("Value cannot be empty", nameof(Value));

    public static T Parse(string s) => Parse(s, provider: null);

    public static bool TryParse([NotNullWhen(true)] string? s, [NotNullWhen(true)] out T? result) =>
        TryParse(s, provider: null, out result);

    public static T Parse(string s, IFormatProvider? provider) =>
        Guid.Parse(s) is Guid guid && guid != Guid.Empty
            ? T.Create(guid)
            : throw new FormatException($"Invalid {typeof(T).Name} format: {s}");

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [NotNullWhen(true)] out T? result
    )
    {
        if (!Guid.TryParse(s, out var guid) || guid == Guid.Empty)
        {
            result = null;
            return false;
        }
        result = T.Create(guid);
        return true;
    }

    public sealed override string ToString() => Value.ToString();
}
