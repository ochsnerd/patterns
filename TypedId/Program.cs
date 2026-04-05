using TypedId;

var guid = Guid.NewGuid();
var fooId = FooId.Parse(guid.ToString());

EntityDictionary<Foo> values = [new() { Id = fooId, Name = "First" }, new() { Id = FooId.NewId(), Name = "Second" }];

foreach (var kvp in values)
{
    Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value.Name}");
}
