using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaCondominio.Domain.Entities;
using SistemaCondominio.Domain.ValueObjects;

namespace SistemaCondominio.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasConversion(
                tenantId => tenantId.Value,
                guid => TenantId.From(guid))
            .IsRequired(); 

            builder.Property(t => t.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(t => t.Document)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(t => t.Document)
                .IsUnique();

            builder.Property(t => t.IsActive)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();
    }
}