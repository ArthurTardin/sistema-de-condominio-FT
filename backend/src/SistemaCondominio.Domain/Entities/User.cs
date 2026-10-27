using SistemaCondominio.Domain.Common;
using SistemaCondominio.Domain.Enums;
using SistemaCondominio.Domain.ValueObjects;

namespace SistemaCondominio.Domain.Entities;

public class User : Entity<Guid>
{
    public TenantId? TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User() { }

    public User(Guid id, TenantId? tenantId, string name, string email, string passwordHash, UserRole role) : base(id)
    {
        TenantId = tenantId;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string newPasswordHash) => PasswordHash = newPasswordHash;
    public void Deactivate() => IsActive = false;
}