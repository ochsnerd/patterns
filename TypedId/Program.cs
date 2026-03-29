using TypedId;

var guid = Guid.NewGuid();
IParsable<Id<Foo>> fooId = Id<Foo>.Parse(guid.ToString());

EntityDictionary<Foo> values =
[
    new() { Id = Id<Foo>.NewId(), Name = "First" },
    new() { Id = Id<Foo>.NewId(), Name = "Second" },
];

Console.WriteLine(fooId);
foreach (var kvp in values)
{
    Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value.Name}");
}
