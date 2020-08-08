using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HandlingErrors.Data.Tests.Infra
{
    public static class DbContextExtensions
    {
        public static void DetachAllEntries(this DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries().ToList())
                context.Entry(entry.Entity).State = EntityState.Detached;
        }
    }
}
