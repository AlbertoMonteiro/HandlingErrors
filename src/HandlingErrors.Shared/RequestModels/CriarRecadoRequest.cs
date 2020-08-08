using MediatR;
using OperationResult;

namespace HandlingErrors.Shared.RequestModels
{
    public class CriarRecadoRequest : IRequest<Result>, IValidatable
    {
        public string Remetente  { get; set; }
        public string Destinatario { get; set; }
        public string Assunto { get; set; }
        public string Mensagem { get; set; }
    }
}
