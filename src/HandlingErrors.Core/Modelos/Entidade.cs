using System;

namespace HandlingErrors.Core.Modelos
{
    public abstract class Entidade
    {
        public long Id { get; protected set; }
        public DateTimeOffset DataCriacao { get; private set; } = DateTimeOffset.UtcNow;
    }
}
