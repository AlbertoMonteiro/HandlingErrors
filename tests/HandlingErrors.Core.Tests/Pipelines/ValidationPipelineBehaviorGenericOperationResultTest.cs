﻿using FluentValidation;
using HandlingErrors.Core.RequestHandlers.Pipelines;
using HandlingErrors.Shared;
using MediatR;
using OperationResult;
using Xunit;

namespace HandlingErrors.Core.Tests.Pipelines;

public class ValidationPipelineBehaviorGenericResultTest
{
    private readonly AbstractValidator<SampleRequestGeneric> _validator;
    private readonly IPipelineBehavior<SampleRequestGeneric, Result<long>> _sut;

    public ValidationPipelineBehaviorGenericResultTest()
    {
        _validator = new SampleRequestValidatorGeneric();
        _sut = new ValidationPipelineBehavior<SampleRequestGeneric, Result<long>>(_validator);
    }

    [Fact]
    public async Task WhenModelContainsErrorsShouldReturnResultWithError()
    {
        //Arrange
        var request = new SampleRequestGeneric();

        //Act
        var (success, result, exception) = await _sut.Handle(request, null, CancellationToken.None);

        //Assert
        Assert.False(success);
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task WhenModelIsValidShouldReturnResultSuccess()
    {
        //Arrange
        var request = new SampleRequestGeneric { Name = "Not null" };

        static Task<Result<long>> Next() => Task.FromResult(Result.Success(1L));

        //Act
        var (success, result, exception) = await _sut.Handle(request, Next, CancellationToken.None);

        //Assert
        Assert.True(success);
        Assert.Null(exception);
        Assert.NotEqual(0, result);
    }
}

public class SampleRequestGeneric : IRequest<Result<long>>, IValidatable
{
    public string Name { get; set; }
}

public class SampleRequestValidatorGeneric : AbstractValidator<SampleRequestGeneric>
{
    public SampleRequestValidatorGeneric()
        => RuleFor(x => x.Name).NotNull();
}
