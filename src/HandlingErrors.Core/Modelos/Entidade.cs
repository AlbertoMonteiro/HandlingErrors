using System;

namespace HandlingErrors.Core.Modelos
{
    public abstract class Entidade
    {
        public virtual long Id { get; protected internal set; }
        public virtual DateTimeOffset DataCriacao { get; protected internal set; } = DateTimeOffset.UtcNow;
    }
}
