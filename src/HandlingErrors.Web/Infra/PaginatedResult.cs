using System.Collections;

namespace HandlingErrors.Web.Infra
{
    public sealed class PaginatedResult
    {
        public PaginatedResult(long? count, IEnumerable allItems) 
            => (Count, Items) = (count, allItems);

        public long? Count { get; set; }
        public IEnumerable Items { get; set; }
    }
}
