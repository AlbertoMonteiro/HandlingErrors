using MediatR;
using OperationResult;

namespace HandlingErrors.Shared.RequestModels;

public record CriarRecadoRequest(string Remetente, string Destinatario, string Assunto, string Mensagem) : IRequest<Result>, IValidatable;
