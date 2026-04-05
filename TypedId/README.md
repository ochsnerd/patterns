# TypedId

## Problem

Raw `Guid` or `string` identifiers lose type information, allowing bugs like:

```csharp
void AssignToTeam(Guid teamId, Guid playerId);

// Compiles fine, but arguments are swapped
AssignToTeam(playerId, teamId);
```

## Solution

Strongly-typed IDs catch these errors at compile time:

```csharp
void AssignToTeam(Id<Team> teamId, Id<Player> playerId);

// Won't compile: Id<Player> cannot be used where Id<Team> is expected
AssignToTeam(playerId, teamId);
```

## Implementation

- `Id<TEntity>` — generic record struct wrapping a `Guid`, implements `IParsable<Id<TEntity>>`
- `IIdentifiable<TSelf>` — interface for entities with an `Id<TSelf>` property
- `EntityDictionary<TEntity>` — store entities based on their id
- `IdJsonConverterFactory` — System.Text.Json serialization support. In a key-context, serializes as a string; otherwise, as an object with a single `Value` property to allow deserialization without a custom converter (e.g. in javascript).

## Usage

Define an entity with a typed ID:

```csharp
public class Order : IIdentifiable<Order>
{
    public required Id<Order> Id { get; init; }
}
```

Use in APIs:

```csharp
public Order GetOrder(Id<Order> id) => ...

var orderId = Id<Order>.NewId();
var parsed = Id<Order>.Parse("550e8400-e29b-41d4-a716-446655440000");
```
