namespace TypedId;

public class Foo : IIdentifiableBy<FooId>
{
    public required FooId Id { get; init; }

    public string Name { get; set; } = string.Empty;
}
