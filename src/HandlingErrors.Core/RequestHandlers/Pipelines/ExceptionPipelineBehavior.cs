using MediatR;
using OperationResult;
using System.Reflection;

namespace HandlingErrors.Core.RequestHandlers.Pipelines;

public sealed class ExceptionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly MethodInfo? _resultError;
    private readonly Type _type = typeof(TResponse);

    public ExceptionPipelineBehavior()
        => _resultError = ResultUtils.GetGenericError(_type);

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next.Invoke();
        }
        catch (Exception e)
        {
            return _type == ResultUtils.TypeResult || _resultError is null
                ? (TResponse)Convert.ChangeType(Result.Error(e), _type)!
                : (TResponse)Convert.ChangeType(_resultError.Invoke(null, new object[] { e }), _type)!;
        }
    }
}
