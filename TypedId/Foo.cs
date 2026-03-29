namespace TypedId;

public class Foo : IIdentifiable<Foo>
{
    public required Id<Foo> Id { get; init; } = Id<Foo>.NewId();

    public string Name { get; set; } = string.Empty;
}
