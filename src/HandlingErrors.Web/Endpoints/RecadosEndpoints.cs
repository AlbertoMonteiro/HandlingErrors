using HandlingErrors.Shared.Exceptions;
using HandlingErrors.Shared.RequestModels;
using HandlingErrors.Shared.ViewModels;
using HandlingErrors.Web.Infra;
using MediatR;

namespace HandlingErrors.Web.Endpoints;

public static class RecadosEndpoints
{
    public static void MapGet(IEndpointRouteBuilder app)
        => app.MapGet("api/recados", async (IMediator mediator, IODataQuery<RecadoViewModel> query) => await mediator.Send(new ObterRecadosRequest()) switch
            {
                (true, var resultado, _) => Results.Ok(query.Apply(resultado!)),
                (_, _, var erro) => TratarErro(erro!)
            }).Produces(200, typeof(RecadoViewModel[]))
            .WithName("Obter Recados")
            .WithTags("Recados")
            .WithMetadata(EnabledODataQuery.Instance);

    public static void MapPost(IEndpointRouteBuilder app)
        => app.MapPost("api/recados", async (IMediator mediator, CriarRecadoRequest request) => await mediator.Send(request) switch
        {
            (true, _) => Results.StatusCode(201),
            (_, var erro) => TratarErro(erro!)
        })
        .WithName("Criar Recado")
        .WithTags("Recados");

    private static IResult TratarErro(Exception erro)
       => erro switch
       {
           ModeloInvalidoException e => Results.BadRequest(e.Erros),
           Exception e => Results.StatusCode(500)
       };
}

public class EnabledODataQuery
{
    private EnabledODataQuery() { }

    public static EnabledODataQuery Instance { get; } = new();
}