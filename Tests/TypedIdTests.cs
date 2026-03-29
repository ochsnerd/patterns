namespace Tests;

using TypedId;

public class TypedIdTests
{
    [Test]
    public void NewIdCreatesUniqueIds()
    {
        var id1 = FooId.NewId();
        var id2 = FooId.NewId();

        id1.Should().NotBe(id2);
    }

    [Test]
    public void CreateWrapsGuid()
    {
        var guid = Guid.NewGuid();
        var id = FooId.Create(guid);

        id.Value.Should().Be(guid);
    }

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
        result.Should().BeNull();
    }

    [Test]
    public void CreateRejectsEmptyGuid()
    {
        var act = () => FooId.Create(Guid.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void TryParseRejectsEmptyGuid()
    {
        var success = FooId.TryParse(Guid.Empty.ToString(), out var result);

        success.Should().BeFalse();
        result.Should().BeNull();
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

        var original = new EntityDictionary<FooId, Foo> { entity1, entity2 };

        var json = System.Text.Json.JsonSerializer.Serialize(original, Json.Options);
        var deserialized = System.Text.Json.JsonSerializer.Deserialize<EntityDictionary<FooId, Foo>>(
            json,
            Json.Options
        );

        deserialized.Should().BeEquivalentTo(original);
    }
}
