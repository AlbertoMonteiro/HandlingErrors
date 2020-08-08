using HandlingErrors.Core.Modelos;
using System.Linq;

namespace HandlingErrors.Core.Repositorios
{
    public interface IRepositorio<T>
        where T : Entidade
    {
        long Adicionar(T entidade);
        void Atualizar(T entidade);
        T ObterPorId(long id);
        T ObterPorIdAsNoTrack(long id); //como o EF usa esse termo AsNoTrack preferi manter o nome em inglês para facilitar o entendimento
        TViewModel ObterProjetado<TViewModel>(long id);
        IQueryable<T> ObterTodos();
        IQueryable<TViewModel> ObterTodosProjetado<TViewModel>(object parametros = null);
        void Remover(long id);
        bool Existe(long id);
    }
}
