# TypedId

## Problem

Raw `Guid` or `string` identifiers lose type information, allowing bugs like:

```csharp
void TransferFunds(Guid fromAccount, Guid toAccount, decimal amount);

// Compiles fine, but arguments are swapped
TransferFunds(toAccountId, fromAccountId, 100m);
```

## Solution

Strongly-typed IDs catch these errors at compile time:

```csharp
void TransferFunds(AccountId from, AccountId to, decimal amount);

// Won't compile: CustomerId cannot be used where AccountId is expected
TransferFunds(customerId, accountId, 100m);
```

## Implementation

- `Id<T>` — base record wrapping a `Guid`, implements `IParsable<T>`
- `EntityDictionary<TId, TEntity>` - store entities based on their id
- `IdJsonConverterFactory` — System.Text.Json serialization support

## Usage

Define a new ID type:

```csharp
public record OrderId(Guid Value) : Id<OrderId>(Value), IIdFactory<OrderId>
{
    public static OrderId Create(Guid value) => new(value);
    public static OrderId NewId() => Create(Guid.NewGuid());
}
```

Use in entities and APIs:

```csharp
public class Order : IIdentifiableBy<OrderId>
{
    public required OrderId Id { get; init; }
}

public Order GetOrder(OrderId id) => ...
```
