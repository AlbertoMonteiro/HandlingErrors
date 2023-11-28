using FluentValidation;
using HandlingErrors.Shared;
using HandlingErrors.Shared.Exceptions;
using MediatR;
using OperationResult;
using System.Reflection;

namespace HandlingErrors.Core.RequestHandlers.Pipelines;

public sealed class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IValidatable, IRequest<TResponse>
{
    private readonly AbstractValidator<TRequest> _validator;
    private readonly MethodInfo? _resultError;
    private readonly Type _type = typeof(TResponse);

    public ValidationPipelineBehavior(AbstractValidator<TRequest> validator)
    {
        _validator = validator;
        _resultError = ResultUtils.GetGenericError(_type);
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        FluentValidation.Results.ValidationResult validationResult;

        validationResult = _validator.Validate(request);
        if (validationResult.IsValid)
            return next.Invoke();
        
        var validationError = new ModeloInvalidoException(validationResult.Errors.GroupBy(v => v.PropertyName, v => v.ErrorMessage).ToDictionary(v => v.Key, v => v.Select(y => y)));

        return _type == ResultUtils.TypeResult || _resultError is null
            ? Task.FromResult((TResponse)Convert.ChangeType(Result.Error(validationError), _type))
            : Task.FromResult((TResponse)Convert.ChangeType(_resultError.Invoke(null, new object[] { validationError }), _type)!);
    }
}
