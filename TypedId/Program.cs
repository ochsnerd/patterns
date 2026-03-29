using TypedId;

var guid = Guid.NewGuid();
IParsable<FooId> fooId = FooId.Parse(guid.ToString());

EntityDictionary<FooId, Foo> values =
[
    new() { Id = FooId.NewId(), Name = "First" },
    new() { Id = FooId.NewId(), Name = "Second" },
];

Console.WriteLine(fooId);
foreach (var kvp in values)
{
    Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value.Name}");
}
