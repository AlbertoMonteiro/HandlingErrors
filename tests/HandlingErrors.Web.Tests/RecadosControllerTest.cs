using HandlingErrors.Shared;
using HandlingErrors.Shared.Exceptions;
using HandlingErrors.Shared.RequestModels;
using HandlingErrors.Shared.ViewModels;
using HandlingErrors.Web.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static OperationResult.Result;

namespace HandlingErrors.Web.Tests
{
    public class RecadosControllerTest
    {
        protected readonly IMediator _mediator;
        private readonly RecadosController _sut;

        public RecadosControllerTest()
        {
            _mediator = Substitute.For<IMediator>();
            _sut = new RecadosController(_mediator);
        }

        [Fact]
        public async Task GetActionDeveEnviarORequestCorretamente()
        {
            //Arrange
            _mediator.Send(Arg.Any<ObterRecadosRequest>())
                .Returns(Success(default(IQueryable<RecadoViewModel>)));

            //Act
            await _sut.Get();

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
            var result = await _sut.Get();

            //Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            await _mediator.Received().Send(Arg.Any<ObterRecadosRequest>());
        }

        [Fact]
        public async Task PostActionDeveEnviarORequestCorretamente()
        {
            //Arrange
            var request = new CriarRecadoRequest();
            _mediator.Send(request)
                .Returns(Success());

            //Act
            await _sut.Post(request);

            //Assert
            await _mediator.Received().Send(request);
        }

        [Fact]
        public async Task PostActionRecebendoUmRequestInvalidoRetornaBadRequest()
        {
            //Arrange
            var request = new CriarRecadoRequest();
            _mediator.Send(request)
              .Returns(Error(new ModeloInvalidoException(null)));

            //Act
            await _sut.Post(request);

            //Assert
            await _mediator.Received().Send(request);
        }
    }
}
