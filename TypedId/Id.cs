#pragma warning disable CA1000
namespace TypedId;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

[JsonConverter(typeof(IdJsonConverterFactory))]
public readonly record struct Id<TEntity>(Guid Value) : IParsable<Id<TEntity>>
    where TEntity : IIdentifiable<TEntity>
{
    public Guid Value { get; } =
        Value != Guid.Empty ? Value : throw new ArgumentException("Value cannot be empty", nameof(Value));

    public static Id<TEntity> NewId() => new(Guid.NewGuid());

    public static Id<TEntity> Parse(string s) => Parse(s, provider: null);

    public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out Id<TEntity> result) =>
        TryParse(s, provider: null, out result);

    public static Id<TEntity> Parse(string s, IFormatProvider? provider) =>
        Guid.Parse(s) is Guid guid && guid != Guid.Empty
            ? new Id<TEntity>(guid)
            : throw new FormatException($"Invalid Id<{typeof(TEntity).Name}> format: {s}");

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out Id<TEntity> result
    )
    {
        if (!Guid.TryParse(s, out var guid) || guid == Guid.Empty)
        {
            result = default;
            return false;
        }
        result = new Id<TEntity>(guid);
        return true;
    }

    public override string ToString() => Value.ToString();
}
