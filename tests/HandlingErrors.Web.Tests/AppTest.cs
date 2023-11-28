using HandlingErrors.Shared.Exceptions;
using HandlingErrors.Shared.RequestModels;
using HandlingErrors.Shared.ViewModels;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Net.Http.Json;
using Xunit;
using static OperationResult.Result;

namespace HandlingErrors.Web.Tests;

public class AppTest
{
    private static readonly TestApplication _application;
    private static readonly HttpClient _client;
    private static readonly IMediator _mediator;

    static AppTest()
    {
        _application = new TestApplication();
        _client = _application.CreateClient();
        _mediator = _application.Services.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task GetActionDeveEnviarORequestCorretamente()
    {
        //Arrange
        _mediator.Send(Arg.Any<ObterRecadosRequest>())
            .Returns(Success(default(IQueryable<RecadoViewModel>)));

        //Act
        var response = await _client.GetAsync("api/recados");

        //Assert
        await _mediator.Received().Send(Arg.Any<ObterRecadosRequest>());
    }

    [Fact]
    public async Task GetActionDeveDevolver500CasoAlgumaExceptionNaoTratadaAconteca()
    {
        //Arrange
        _mediator.Send(Arg.Any<ObterRecadosRequest>())
            .Returns(Error<IQueryable<RecadoViewModel>>(new Exception()));

        //Act
        var response = await _client.GetAsync("api/recados");

        //Assert
        Assert.Equal(500, (int)response.StatusCode);
        await _mediator.Received().Send(Arg.Any<ObterRecadosRequest>());
    }

    [Fact]
    public async Task PostActionDeveEnviarORequestCorretamente()
    {
        //Arrange
        var request = new CriarRecadoRequest("", "", "", "");
        _mediator.Send(request)
            .Returns(Success());

        //Act
        var response = await _client.PostAsJsonAsync("api/recados", request);

        //Assert
        await _mediator.Received().Send(request);
    }

    [Fact]
    public async Task PostActionRecebendoUmRequestInvalidoRetornaBadRequest()
    {
        //Arrange
        var request = new CriarRecadoRequest("", "", "", "");
        _mediator.Send(request)
          .Returns(Error(new ModeloInvalidoException(null)));

        //Act
        var response = await _client.PostAsJsonAsync("api/recados", request);

        //Assert
        await _mediator.Received().Send(request);
    }
}
