using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HandlingErrors.Core.Repositorios;
using HandlingErrors.Shared.RequestModels;
using HandlingErrors.Shared.ViewModels;
using MediatR;
using OperationResult;

namespace HandlingErrors.Core.RequestHandlers
{
    public class ObterRecadosRequestHandler :
        IRequestHandler<ObterRecadosRequest, Result<IQueryable<RecadoViewModel>>>,
        IRequestHandler<ObterRecadosComNHibernateRequest, Result<IQueryable<RecadoViewModel>>>
    {
        private readonly IRecadoRepositorio _recados;
        private readonly IRecadoRepositorioNHibernate _repositorioNHibernate;

        public ObterRecadosRequestHandler(IRecadoRepositorio recados, IRecadoRepositorioNHibernate repositorioNHibernate)
        {
            _recados = recados;
            _repositorioNHibernate = repositorioNHibernate;
        }

        public Task<Result<IQueryable<RecadoViewModel>>> Handle(ObterRecadosRequest request, CancellationToken cancellationToken)
            => Result.Success(_recados.ObterTodosProjetado<RecadoViewModel>()).AsTask;

        public Task<Result<IQueryable<RecadoViewModel>>> Handle(ObterRecadosComNHibernateRequest request, CancellationToken cancellationToken)
            => Result.Success(_repositorioNHibernate.ObterTodosProjetado<RecadoViewModel>()).AsTask;
    }
}
