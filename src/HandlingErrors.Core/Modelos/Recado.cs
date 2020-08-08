using System.Collections.Generic;

namespace HandlingErrors.Core.Modelos
{
    public class Recado : Entidade
    {
        public Recado(long id, string remetente, string destinatario, string assunto, string mensagem, long? agrupadoComId = null)
            : this(remetente, destinatario, assunto, mensagem, agrupadoComId)
            => Id = id;

        public Recado(string remetente, string destinatario, string assunto, string mensagem, long? agrupadoComId = null)
        {
            Remetente = remetente;
            Destinatario = destinatario;
            Assunto = assunto;
            Mensagem = mensagem;
            AgrupadoComId = agrupadoComId;
        }

        public string Remetente { get; private set; }
        public string Destinatario { get; private set; }
        public string Assunto { get; private set; }
        public string Mensagem { get; private set; }
        public List<Recado> RecadosFilhos { get; set; }
        public long? AgrupadoComId { get; private set; }
        public Recado AgrupadoCom { get; private set; }
    }
}
