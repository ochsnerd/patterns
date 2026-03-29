namespace TypedId;

public interface IIdentifiable<TSelf>
    where TSelf : IIdentifiable<TSelf>
{
    Id<TSelf> Id { get; }
}
