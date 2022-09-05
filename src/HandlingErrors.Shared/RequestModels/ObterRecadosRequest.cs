using HandlingErrors.Shared.ViewModels;
using MediatR;
using OperationResult;

namespace HandlingErrors.Shared.RequestModels;

public class ObterRecadosRequest : IRequest<Result<IQueryable<RecadoViewModel>>>
{

}
