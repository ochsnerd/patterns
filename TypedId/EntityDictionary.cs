namespace TypedId;

public class EntityDictionary<TId, TEntity> : Dictionary<TId, TEntity>
    where TEntity : IIdentifiableBy<TId>
    where TId : Id<TId>, IIdFactory<TId>
{
    public void Add(TEntity entity) => Add(entity.Id, entity);

    public bool Remove(TEntity entity) => Remove(entity.Id);
}
