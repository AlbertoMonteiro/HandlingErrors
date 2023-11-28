using FluentValidation.TestHelper;
using HandlingErrors.Core.Modelos.Validations;
using HandlingErrors.Shared.RequestModels;
using Xunit;

namespace HandlingErrors.Core.Tests.Validations;

public class ValidadorCriarRecadoRequestTest
{
    private readonly ValidadorCriarRecadoRequest _sut = new();

    [Fact]
    public void UmModeloValidoNaoGeraErros()
    {
        //arrenge
        var request = new CriarRecadoRequest
        (
            "Fulano",
            "Ciclano",
            "Mensagem urgente",
            "Esse modelo é válido!"
        );

        //act
        var resultado = _sut.TestValidate(request);

        //assert
        resultado.ShouldNotHaveValidationErrorFor(x => x.Remetente);
        resultado.ShouldNotHaveValidationErrorFor(x => x.Destinatario);
        resultado.ShouldNotHaveValidationErrorFor(x => x.Assunto);
        resultado.ShouldNotHaveValidationErrorFor(x => x.Mensagem);
    }

    [Fact]
    public void NaoDeveAceitarCamposVazios()
    {
        //arrenge
        var request = new CriarRecadoRequest(null!, null!, null!, null!);

        //act
        var resultado = _sut.TestValidate(request);

        //assert
        resultado.ShouldHaveValidationErrorFor(x => x.Remetente).WithErrorCode("NotEmptyValidator");
        resultado.ShouldHaveValidationErrorFor(x => x.Destinatario).WithErrorCode("NotEmptyValidator");
        resultado.ShouldHaveValidationErrorFor(x => x.Assunto).WithErrorCode("NotEmptyValidator");
        resultado.ShouldHaveValidationErrorFor(x => x.Mensagem).WithErrorCode("NotEmptyValidator");
    }

    [Fact]
    public void DeveRespeitarOTamanhoMaximoDeCadaCampo()
    {
        //arrenge
        var request = new CriarRecadoRequest(new string('a', 51),
                                             new string('a', 51),
                                             new string('a', 101),
                                             new string('a', 501));


        //act
        var resultado = _sut.TestValidate(request);

        //assert
        resultado.ShouldHaveValidationErrorFor(x => x.Remetente).WithErrorCode("MaximumLengthValidator");
        resultado.ShouldHaveValidationErrorFor(x => x.Destinatario).WithErrorCode("MaximumLengthValidator");
        resultado.ShouldHaveValidationErrorFor(x => x.Assunto).WithErrorCode("MaximumLengthValidator");
        resultado.ShouldHaveValidationErrorFor(x => x.Mensagem).WithErrorCode("MaximumLengthValidator");
    }
}
