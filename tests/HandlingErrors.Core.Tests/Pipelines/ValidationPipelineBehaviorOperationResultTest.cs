﻿using FluentValidation;
using HandlingErrors.Core.RequestHandlers.Pipelines;
using HandlingErrors.Shared;
using MediatR;
using OperationResult;
using Xunit;

namespace HandlingErrors.Core.Tests.Pipelines;

public class ValidationPipelineBehaviorResultTest
{
    private readonly AbstractValidator<SampleRequest> _validator;
    private readonly IPipelineBehavior<SampleRequest, Result> _sut;

    public ValidationPipelineBehaviorResultTest()
    {
        _validator = new SampleRequestValidator();
        _sut = new ValidationPipelineBehavior<SampleRequest, Result>(_validator);
    }

    [Fact]
    public async Task WhenModelContainsErrorsShouldReturnResultWithError()
    {
        //Arrange
        var request = new SampleRequest();

        //Act
        var (success, exception) = await _sut.Handle(request, null, CancellationToken.None);

        //Assert
        Assert.False(success);
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task WhenModelIsValidShouldReturnResultSuccess()
    {
        //Arrange
        var request = new SampleRequest { Name = "Not null" };

        static Task<Result> Next() => Task.FromResult(Result.Success());

        //Act
        var (success, exception) = await _sut.Handle(request, Next, CancellationToken.None);

        //Assert
        Assert.True(success);
        Assert.Null(exception);
    }
}

public class SampleRequest : IRequest<Result>, IValidatable
{
    public string Name { get; set; }
}

public class SampleRequestValidator : AbstractValidator<SampleRequest>
{
    public SampleRequestValidator()
        => RuleFor(x => x.Name).NotNull();
}
