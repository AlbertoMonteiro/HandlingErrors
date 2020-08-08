using HandlingErrors.Core.Modelos;
using HandlingErrors.Core.Repositorios;
using HandlingErrors.Core.RequestHandlers;
using HandlingErrors.Shared.RequestModels;
using NSubstitute;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HandlingErrors.Core.Tests.RequestHandlers
{
    public class CriarRecadoRequestHandlerTest
    {
        private readonly IRecadoRepositorio _repo;
        private readonly CriarRecadoRequestHandler _sut;

        /* Regras
        * Que tenha remetente (De) ou destinatario (Para) igual ao remetente do recado cadastrado.
        * Que tenha sido registrado a no máximo 6 meses.
        * Que contenha o mesmo assunto, mas ignorando se ele iniciar com:
        *   - referente:
        *   - respondendo:
        *   - complementando:
        * Os recados devem apontar o relacionamento para o primeiro recado do grupo, mesmo que o encontrado na regra seja o mais recente.
        */
        public CriarRecadoRequestHandlerTest()
        {
            _repo = Substitute.For<IRecadoRepositorio>();
            _sut = new CriarRecadoRequestHandler(_repo);
        }

        [Theory]
        [MemberData(nameof(Recados))]
        public async Task CriacaoDeRecadoDeveAgruparQuandoForNecessario(Recado novoRecado, bool deveAgrupar)
        {
            //arrange
            var recadoExistente = new Recado(1L, "Fulano", "Ciclano", "Primeiro recado", "");
            _repo.ObterRecadoParaAgrupamento(novoRecado.Remetente, novoRecado.Destinatario, novoRecado.Assunto)
                .Returns(deveAgrupar ? recadoExistente : default);

            var request = new CriarRecadoRequest
            {
                Remetente = novoRecado.Remetente,
                Destinatario = novoRecado.Destinatario,
                Mensagem = novoRecado.Mensagem,
                Assunto = novoRecado.Assunto
            };

            //act
            var resultado = await _sut.Handle(request, CancellationToken.None);

            //assert
            if (deveAgrupar)
                _repo.Received().Adicionar(Arg.Is<Recado>(r => r.AgrupadoComId == recadoExistente.Id));

            Assert.True(resultado.IsSuccess);
        }

        [Fact]
        public async Task CriacaoDeRecadoDeveAgruparQuandoForNecessarioReferenciandoORecadoRaiz()
        {
            //arrange
            var recadoExistente = new Recado(5, "Fulano", "Ciclano", "referente: Primeiro recado", "", 1);
            var novoRecado = new Recado(7, "Fulano", "Deltano", "Primeiro recado", "", 1);

            _repo.ObterRecadoParaAgrupamento(novoRecado.Remetente, novoRecado.Destinatario, novoRecado.Assunto)
                .Returns(recadoExistente);

            var request = new CriarRecadoRequest
            {
                Remetente = novoRecado.Remetente,
                Destinatario = novoRecado.Destinatario,
                Mensagem = novoRecado.Mensagem,
                Assunto = novoRecado.Assunto
            };

            //act
            var resultado = await _sut.Handle(request, CancellationToken.None);

            //assert
            _repo.Received().Adicionar(Arg.Is<Recado>(r => r.AgrupadoComId == recadoExistente.AgrupadoComId));

            Assert.True(resultado.IsSuccess);
        }

        public static IEnumerable<object[]> Recados()
        {
            yield return new object[] { new Recado(2, "Ciclano", "Fulano", "respondendo: Primeiro recado", "", 1), true };
            yield return new object[] { new Recado(3, "Ciclano", "Fulano", "Segundo recado", ""), false };
            yield return new object[] { new Recado(4, "Ciclano", "Fulano", "complementando: Primeiro recado", "", 1), true };
            yield return new object[] { new Recado(5, "Fulano", "Ciclano", "referente: Primeiro recado", "", 1), true };
            yield return new object[] { new Recado(6, "Fulano", "Ciclano", "respondendo: Segundo recado", ""), false };
            yield return new object[] { new Recado(7, "Fulano", "Deltano", "Primeiro recado", "", 1), true };
            yield return new object[] { new Recado(8, "Deltano", "Fulano", "Primeiro recado", "", 1), true };
            yield return new object[] { new Recado(9, "Fulano", "Deltano", "Primeiro recado", ""), false };
        }
    }
}
