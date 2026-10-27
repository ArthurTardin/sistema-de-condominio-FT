namespace SistemaCondominio.Domain.ValueObjects;

public record TenantId(Guid Value)
{
    public static TenantId Unique() => new(Guid.NewGuid());
    public static TenantId From(Guid value) => new(value);
}