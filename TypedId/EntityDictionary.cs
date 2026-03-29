namespace TypedId;

public class EntityDictionary<TEntity> : Dictionary<Id<TEntity>, TEntity>
    where TEntity : IIdentifiable<TEntity>
{
    public void Add(TEntity entity) => Add(entity.Id, entity);

    public bool Remove(TEntity entity) => Remove(entity.Id);
}
