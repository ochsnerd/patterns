#pragma warning disable SA1200

global using FooId = TypedId.Id<TypedId.Foo>;

namespace TypedId;

public class Foo : IIdentifiable<Foo>
{
    public required FooId Id { get; init; } = Id<Foo>.NewId();

    public string Name { get; set; } = string.Empty;
}
