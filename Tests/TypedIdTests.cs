namespace Tests;

using TypedId;
using FooId = TypedId.Id<TypedId.Foo>;

public class TypedIdTests
{
    [Test]
    public void ParseRoundTrips()
    {
        var original = FooId.NewId();
        var parsed = FooId.Parse(original.ToString());

        parsed.Should().Be(original);
    }

    [Test]
    public void TryParseSucceedsForValidGuid()
    {
        var guid = Guid.NewGuid();

        var success = FooId.TryParse(guid.ToString(), out var result);

        success.Should().BeTrue();
        result!.Value.Should().Be(guid);
    }

    [Test]
    public void TryParseFailsForInvalidInput()
    {
        var success = FooId.TryParse("not-a-guid", out var result);

        success.Should().BeFalse();
        result.Should().Be(default(FooId));
    }

    [Test]
    public void ConstructorRejectsEmptyGuid()
    {
        var act = () => new FooId(Guid.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void TryParseRejectsEmptyGuid()
    {
        var success = FooId.TryParse(Guid.Empty.ToString(), out var result);

        success.Should().BeFalse();
        result.Should().Be(default(FooId));
    }

    [Test]
    public void DictionaryWithIdKeysAndValuesRoundTrips()
    {
        var key1 = FooId.NewId();
        var key2 = FooId.NewId();
        var value1 = FooId.NewId();
        var value2 = FooId.NewId();

        var original = new Dictionary<FooId, FooId> { [key1] = value1, [key2] = value2 };

        var json = System.Text.Json.JsonSerializer.Serialize(original, Json.Options);
        var deserialized = System.Text.Json.JsonSerializer.Deserialize<Dictionary<FooId, FooId>>(json, Json.Options);

        deserialized.Should().BeEquivalentTo(original);
    }

    [Test]
    public void EntityDictionaryRoundTrips()
    {
        var entity1 = new Foo { Id = FooId.NewId(), Name = "Alice" };
        var entity2 = new Foo { Id = FooId.NewId(), Name = "Bob" };

        var original = new EntityDictionary<Foo> { entity1, entity2 };

        var json = System.Text.Json.JsonSerializer.Serialize(original, Json.Options);
        var deserialized = System.Text.Json.JsonSerializer.Deserialize<EntityDictionary<Foo>>(json, Json.Options);

        deserialized.Should().BeEquivalentTo(original);
    }
}
