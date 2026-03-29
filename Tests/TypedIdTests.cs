namespace Tests;

using TypedId;

public class TypedIdTests
{
    [Test]
    public void ParseRoundTrips()
    {
        var original = Id<Foo>.NewId();
        var parsed = Id<Foo>.Parse(original.ToString());

        parsed.Should().Be(original);
    }

    [Test]
    public void TryParseSucceedsForValidGuid()
    {
        var guid = Guid.NewGuid();

        var success = Id<Foo>.TryParse(guid.ToString(), out var result);

        success.Should().BeTrue();
        result!.Value.Should().Be(guid);
    }

    [Test]
    public void TryParseFailsForInvalidInput()
    {
        var success = Id<Foo>.TryParse("not-a-guid", out var result);

        success.Should().BeFalse();
        result.Should().Be(default(Id<Foo>));
    }

    [Test]
    public void ConstructorRejectsEmptyGuid()
    {
        var act = () => new Id<Foo>(Guid.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void TryParseRejectsEmptyGuid()
    {
        var success = Id<Foo>.TryParse(Guid.Empty.ToString(), out var result);

        success.Should().BeFalse();
        result.Should().Be(default(Id<Foo>));
    }

    [Test]
    public void DictionaryWithIdKeysAndValuesRoundTrips()
    {
        var key1 = Id<Foo>.NewId();
        var key2 = Id<Foo>.NewId();
        var value1 = Id<Foo>.NewId();
        var value2 = Id<Foo>.NewId();

        var original = new Dictionary<Id<Foo>, Id<Foo>> { [key1] = value1, [key2] = value2 };

        var json = System.Text.Json.JsonSerializer.Serialize(original, Json.Options);
        var deserialized = System.Text.Json.JsonSerializer.Deserialize<Dictionary<Id<Foo>, Id<Foo>>>(
            json,
            Json.Options
        );

        deserialized.Should().BeEquivalentTo(original);
    }

    [Test]
    public void EntityDictionaryRoundTrips()
    {
        var entity1 = new Foo { Id = Id<Foo>.NewId(), Name = "Alice" };
        var entity2 = new Foo { Id = Id<Foo>.NewId(), Name = "Bob" };

        var original = new EntityDictionary<Foo> { entity1, entity2 };

        var json = System.Text.Json.JsonSerializer.Serialize(original, Json.Options);
        var deserialized = System.Text.Json.JsonSerializer.Deserialize<EntityDictionary<Foo>>(json, Json.Options);

        deserialized.Should().BeEquivalentTo(original);
    }
}
