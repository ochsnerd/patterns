#pragma warning disable CA1000

namespace TypedId;

public interface IIdFactory<out T>
    where T : IIdFactory<T>
{
    static abstract T Create(Guid value);
}
