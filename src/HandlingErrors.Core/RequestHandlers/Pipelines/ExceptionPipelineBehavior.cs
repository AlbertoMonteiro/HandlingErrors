using MediatR;
using OperationResult;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace HandlingErrors.Core.RequestHandlers.Pipelines
{
    public sealed class ExceptionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly MethodInfo _ResultError;
        private readonly Type _type = typeof(TResponse);
        private readonly Type _typeResult = typeof(Result);

        public ExceptionPipelineBehavior()
        {
            if (_type.IsGenericType)
            {
                _ResultError = _typeResult.GetMethods().FirstOrDefault(m => m.Name == "Error" && m.IsGenericMethod);
                _ResultError = _ResultError.MakeGenericMethod(_type.GetGenericArguments().First());
            }
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return await next?.Invoke();
            }
            catch (Exception e)
            {
                return _type == _typeResult
                    ? (TResponse)Convert.ChangeType(Result.Error(e), _type)
                    : (TResponse)Convert.ChangeType(_ResultError.Invoke(null, new object[] { e }), _type);
            }
        }
    }
}
