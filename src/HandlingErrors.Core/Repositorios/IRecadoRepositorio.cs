using HandlingErrors.Core.Modelos;

namespace HandlingErrors.Core.Repositorios
{
    public interface IRecadoRepositorio : IRepositorio<Recado>
    {
        Recado ObterRecadoParaAgrupamento(string rementente, string destinatario, string assunto);
    }
}
