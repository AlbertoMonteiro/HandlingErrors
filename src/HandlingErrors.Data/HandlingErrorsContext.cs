using HandlingErrors.Core.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HandlingErrors.Data
{
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
}
