using HandlingErrors.Shared.ViewModels;
using MediatR;
using OperationResult;
using System.Linq;

namespace HandlingErrors.Shared.RequestModels
{
    public class ObterRecadosRequest : IRequest<Result<IQueryable<RecadoViewModel>>>
    {

    }
}
