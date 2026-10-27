namespace SistemaCondominio.Domain.Common;

public abstract class Entity<TId> where TId : notnull
{
    public TId Id { get; protected set; } = default!;

    protected Entity(TId id)
    {
        Id = id;
    }

    protected Entity() { }
}