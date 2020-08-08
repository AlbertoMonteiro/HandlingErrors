using System;
using System.Collections.Generic;

namespace HandlingErrors.Shared.Exceptions
{
    public sealed class ModeloInvalidoException : Exception
    {
        public ModeloInvalidoException(Dictionary<string, IEnumerable<string>> erros)
            : base("Dados inválidos")
            => Erros = erros;

        public Dictionary<string, IEnumerable<string>> Erros { get; }
    }
}
