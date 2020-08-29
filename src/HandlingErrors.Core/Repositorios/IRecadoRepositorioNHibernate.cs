using HandlingErrors.Core.Modelos;

namespace HandlingErrors.Core.Repositorios
{
    public interface IRecadoRepositorioNHibernate : IRepositorio<Recado>
    {
        Recado ObterRecadoParaAgrupamento(string rementente, string destinatario, string assunto);
    }
}
