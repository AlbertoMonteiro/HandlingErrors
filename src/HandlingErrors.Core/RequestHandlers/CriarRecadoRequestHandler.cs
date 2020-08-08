using HandlingErrors.Core.Modelos;
using HandlingErrors.Core.Repositorios;
using HandlingErrors.Shared;
using HandlingErrors.Shared.RequestModels;
using MediatR;
using OperationResult;
using System.Threading;
using System.Threading.Tasks;

namespace HandlingErrors.Core.RequestHandlers
{
    public class CriarRecadoRequestHandler : IRequestHandler<CriarRecadoRequest, Result>
    {
        private readonly IRecadoRepositorio _recados;

        public CriarRecadoRequestHandler(IRecadoRepositorio recados) 
            => _recados = recados;

        public Task<Result> Handle(CriarRecadoRequest request, CancellationToken cancellationToken)
        {
            var recadoPai = _recados.ObterRecadoParaAgrupamento(request.Remetente, request.Destinatario, request.Assunto);

            var recado = new Recado(request.Remetente, request.Destinatario, request.Assunto, request.Mensagem, recadoPai?.AgrupadoComId ?? recadoPai?.Id);

            _recados.Adicionar(recado);

            return Result.Success().AsTask;
        }
    }
}
