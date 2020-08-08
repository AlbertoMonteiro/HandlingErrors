using HandlingErrors.Core.Repositorios;
using HandlingErrors.Shared;
using HandlingErrors.Shared.RequestModels;
using HandlingErrors.Shared.ViewModels;
using MediatR;
using OperationResult;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HandlingErrors.Core.RequestHandlers
{
    public class ObterRecadosRequestHandler : IRequestHandler<ObterRecadosRequest, Result<IQueryable<RecadoViewModel>>>
    {
        private readonly IRecadoRepositorio _recados;

        public ObterRecadosRequestHandler(IRecadoRepositorio recados)
            => _recados = recados;

        public Task<Result<IQueryable<RecadoViewModel>>> Handle(ObterRecadosRequest request, CancellationToken cancellationToken) 
            => Result.Success(_recados.ObterTodosProjetado<RecadoViewModel>()).AsTask;
    }
}
