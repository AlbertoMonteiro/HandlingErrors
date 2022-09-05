using HandlingErrors.Core.Modelos;
using Microsoft.EntityFrameworkCore;

namespace HandlingErrors.Data;

/// <summary>
/// Contexto para acesso a base de dados
/// </summary>
/// <C4Technology>MS SQL Server</C4Technology>
/// <C4Tags>mssqlserver,database</C4Tags>
/// <C4RelationshipLabel>Uses</C4RelationshipLabel>
/// <C4RelationshipProtocol>Entity Framework Core</C4RelationshipProtocol>
public class HandlingErrorsContext : DbContext
{
    public HandlingErrorsContext(DbContextOptions<HandlingErrorsContext> options)
        : base(options)
    {

    }

    public DbSet<Recado> Recados { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var baseType = typeof(IEntityTypeConfiguration<>);

        modelBuilder.UseIdentityColumns();

        var baseStringType = typeof(string);
        modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == baseStringType)
            .Select(p => modelBuilder.Entity(p.DeclaringEntityType.ClrType).Property(p.Name))
            .ToList()
            .ForEach(propBuilder => propBuilder.IsRequired().IsUnicode(false).HasMaxLength(255));

        typeof(HandlingErrorsContext)
            .Assembly
            .GetTypes()
            .Where(t => t.GetInterfaces().Any(gi => gi.IsGenericType && gi.GetGenericTypeDefinition() == baseType))
            .Select(t => (dynamic)Activator.CreateInstance(t))
            .ToList()
            .ForEach(configurationInstance => modelBuilder.ApplyConfiguration(configurationInstance));

        base.OnModelCreating(modelBuilder);
    }
}
