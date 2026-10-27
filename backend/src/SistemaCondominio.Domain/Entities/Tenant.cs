using SistemaCondominio.Domain.Common;
using SistemaCondominio.Domain.ValueObjects;

namespace SistemaCondominio.Domain.Entities;

public class Tenant : Entity<TenantId>
{
    public string Name { get; private set; } = string.Empty;
    public string Document { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Tenant() { }

    public Tenant(TenantId id, string name, string document) : base(id)
    {
        Name = name;
        Document = document;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}