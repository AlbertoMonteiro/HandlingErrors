using System.Collections.Generic;

namespace HandlingErrors.Core.Modelos
{
    public class Recado : Entidade
    {
        protected Recado() { } //nhibernate needs

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

        public virtual string Remetente { get; protected internal set; }
        public virtual string Destinatario { get; protected internal set; }
        public virtual string Assunto { get; protected internal set; }
        public virtual string Mensagem { get; protected internal set; }
        public virtual List<Recado> RecadosFilhos { get; set; }
        public virtual long? AgrupadoComId { get; protected internal set; }
        public virtual Recado AgrupadoCom { get; protected internal set; }
    }
}
