using HandlingErrors.Shared.Exceptions;
using HandlingErrors.Shared.RequestModels;
using HandlingErrors.Shared.ViewModels;
using HandlingErrors.Web.Infra;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HandlingErrors.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecadosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RecadosController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [EnableQueryCustom]
        [ProducesResponseType(200, Type = typeof(RecadoViewModel[]))]
        public async Task<IActionResult> Get()
            => await _mediator.Send(new ObterRecadosRequest()) switch
            {
                (true, var resultado, _) => Ok(resultado),
                (_, _, var erro) => TratarErro(erro)
            };

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post(CriarRecadoRequest request)
            => await _mediator.Send(request) switch
            {
                (true, _) => StatusCode(201),
                (_, var erro) => TratarErro(erro)
            };

        [NonAction]
        private IActionResult TratarErro(Exception erro)
           => erro switch
           {
               ModeloInvalidoException e => BadRequest(e.Erros),
               Exception e => Return500AndLogError(e)
           };

        [NonAction]
        private static StatusCodeResult Return500AndLogError(Exception e)
        {
            Console.WriteLine(e.ToString());
            return new StatusCodeResult(500);
        }
    }
}