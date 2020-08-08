using FluentValidation;
using FluentValidation.Results;
using HandlingErrors.Shared;
using HandlingErrors.Shared.Exceptions;
using MediatR;
using OperationResult;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace HandlingErrors.Core.RequestHandlers.Pipelines
{
    public sealed class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IValidatable
    {
        private readonly AbstractValidator<TRequest> _validator;
        private readonly MethodInfo _ResultError;
        private readonly Type _type = typeof(TResponse);
        private readonly Type _typeResultGeneric = typeof(Result<>);
        private readonly Type _typeResult = typeof(Result);

        public ValidationPipelineBehavior(AbstractValidator<TRequest> validator)
        {
            _validator = validator;
            if (_type.IsGenericType)
            {
                _ResultError = _typeResult.GetMethods().FirstOrDefault(m => m.Name == "Error" && m.IsGenericMethod);
                _ResultError = _ResultError.MakeGenericMethod(_type.GetGenericArguments().First());
            }
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            ValidationResult validationResult;
            ModeloInvalidoException validationError;
            if (_type == _typeResult)
            {
                validationResult = _validator.Validate(request);
                if (validationResult.IsValid)
                    return next?.Invoke();

                validationError = new ModeloInvalidoException(validationResult.Errors.GroupBy(v => v.PropertyName, v => v.ErrorMessage).ToDictionary(v => v.Key, v => v.Select(y => y)));
                return Task.FromResult((TResponse)Convert.ChangeType(Result.Error(validationError), _type));
            }

            if (!_type.IsGenericType || _type.GetGenericTypeDefinition() != _typeResultGeneric)
                return next?.Invoke();

            validationResult = _validator.Validate(request);
            if (validationResult.IsValid)
                return next?.Invoke();

            validationError = new ModeloInvalidoException(validationResult.Errors.GroupBy(v => v.PropertyName, v => v.ErrorMessage).ToDictionary(v => v.Key, v => v.Select(y => y)));
            return Task.FromResult((TResponse)Convert.ChangeType(_ResultError.Invoke(null, new object[] { validationError }), _type));
        }
    }
}
