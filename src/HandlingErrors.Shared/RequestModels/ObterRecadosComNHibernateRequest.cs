using System.Linq;
using HandlingErrors.Shared.ViewModels;
using MediatR;
using OperationResult;

namespace HandlingErrors.Shared.RequestModels
{
    public class ObterRecadosComNHibernateRequest : IRequest<Result<IQueryable<RecadoViewModel>>>
    {

    }
}
