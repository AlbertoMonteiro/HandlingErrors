using System;

namespace HandlingErrors.Shared.ViewModels
{
    public sealed class RecadoViewModel
    {
        public long Id { get; set; }
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public string Assunto { get; set; }
        public string Mensagem { get; set; }
        public DateTimeOffset DataCriacao { get; set; } 
        public long? AgrupadoComId { get; set; }
        public int TotalFilhos{ get; set; }
    }
}
