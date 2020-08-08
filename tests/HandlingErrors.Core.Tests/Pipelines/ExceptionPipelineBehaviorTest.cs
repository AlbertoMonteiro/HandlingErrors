using HandlingErrors.Core.RequestHandlers.Pipelines;
using HandlingErrors.Shared;
using OperationResult;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HandlingErrors.Core.Tests.Pipelines
{
    public class ExceptionPipelineBehaviorTest
    {
        private readonly ExceptionPipelineBehavior<SampleRequestGeneric, Result<long>> _sut;

        public ExceptionPipelineBehaviorTest()
            => _sut = new ExceptionPipelineBehavior<SampleRequestGeneric, Result<long>>();

        [Fact]
        public async Task WhenHandlerThrowsAnExceptionPipelineConvertToResultWithError()
        {
            //Arrange
            static Task<Result<long>> Next() => throw new Exception("Fail");

            //Act
            var result = await _sut.Handle(null, CancellationToken.None, Next);

            //Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task WhenHandlerDoesNotThrowsAnExceptionPipelineDoesNotChangeResult()
        {
            //Arrange
            static Task<Result<long>> Next() => Task.FromResult(Result.Success(1L));

            //Act
            var result = await _sut.Handle(null, CancellationToken.None, Next);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1L, result.Value);
        }
    }
}
