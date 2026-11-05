using Microsoft.EntityFrameworkCore;
using SistemaCondominio.Domain.Entities;

namespace SistemaCondominio.Infrastructure.Persistence.Context;

public class AppDbContext : DbContext
{
    // Construtor principal exigido pelo EF Core e Injeção de Dependências
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}