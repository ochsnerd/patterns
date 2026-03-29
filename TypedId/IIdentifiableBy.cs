namespace TypedId;

public interface IIdentifiableBy<T>
    where T : Id<T>, IIdFactory<T>
{
    T Id { get; }
}
