using HandlingErrors.Shared.RequestModels;
using FluentValidation;

namespace HandlingErrors.Core.Modelos.Validations
{
    public sealed class ValidadorCriarRecadoRequest : AbstractValidator<CriarRecadoRequest>
    {
        public ValidadorCriarRecadoRequest()
        {
            RuleFor(r => r.Remetente).NotEmpty().MaximumLength(50);
            RuleFor(r => r.Destinatario).NotEmpty().MaximumLength(50);
            RuleFor(r => r.Assunto).NotEmpty().MaximumLength(100);
            RuleFor(r => r.Mensagem).NotEmpty().MaximumLength(500);
        }
    }
}
