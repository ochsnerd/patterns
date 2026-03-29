namespace TypedId;

public record FooId(Guid Value) : Id<FooId>(Value), IIdFactory<FooId>
{
    public static FooId Create(Guid value) => new(value);

    public static FooId NewId() => Create(Guid.NewGuid());
}
