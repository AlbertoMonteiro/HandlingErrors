using HandlingErrors.Core.Modelos;
using HandlingErrors.Data.Tests.Infra;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Reflection;
using Xunit;

namespace HandlingErrors.Data.Tests
{
    [Collection("Database collection")]
    public class RecadoRepositorioTests : IDisposable
    {
        private readonly HandlingErrorsContext _ctx;
        private readonly IDbContextTransaction _transaction;
        private readonly RecadoRepositorio _sut;

        public RecadoRepositorioTests(DatabaseFixture fixture)
        {
            _ctx = fixture.Context;
            _transaction = _ctx.Database.BeginTransaction();
            _sut = new RecadoRepositorio(fixture.Context, fixture.Mapper);
        }

        [Fact]
        public void ObterRecadoParaAgrupamentoRetornarUmRecadoQuandoExistir()
        {
            //arrange
            var recadoExistente = new Recado("Fulano", "Ciclano", "Primeiro recado", "Msg");
            _sut.Adicionar(recadoExistente);
            _ctx.DetachAllEntries();
            var recadoInserir = new Recado("Ciclano", "Fulano", "respondendo: Primeiro recado", "");

            //act
            var recado = _sut.ObterRecadoParaAgrupamento(recadoInserir.Remetente, recadoInserir.Destinatario, recadoInserir.Assunto);

            //assert
            Assert.NotNull(recado);
            Assert.Equal(recadoExistente.Destinatario, recado.Destinatario);
            Assert.Equal(recadoExistente.Remetente, recado.Remetente);
            Assert.Equal(recadoExistente.Assunto, recado.Assunto);
            Assert.Equal(recadoExistente.Id, recado.Id);
            Assert.Equal(recadoExistente.Mensagem, recado.Mensagem);
        }

        [Fact]
        public void ObterRecadoParaAgrupamentoRetornarUmRecadoQuandoExistirMesmoNaoSendoOPrincipal()
        {
            //arrange
            var recadoExistente = new Recado("Fulano", "Ciclano", "Primeiro recado", "Msg");
            AlterarPropriedadeComReflection(recadoExistente, nameof(Recado.DataCriacao), DateTimeOffset.UtcNow.AddDays(-7 * 30));
            _sut.Adicionar(recadoExistente);
            var outroRecadoExistente = new Recado("Ciclano", "Fulano", "respondendo: Primeiro recado", "Msg", recadoExistente.Id);
            AlterarPropriedadeComReflection(outroRecadoExistente, nameof(Recado.DataCriacao), DateTimeOffset.UtcNow.AddDays(-5 * 30));
            _sut.Adicionar(outroRecadoExistente);
            _ctx.DetachAllEntries();
            var recadoInserir = new Recado("Ciclano", "Fulano", "respondendo: Primeiro recado", "");

            //act
            var recado = _sut.ObterRecadoParaAgrupamento(recadoInserir.Remetente, recadoInserir.Destinatario, recadoInserir.Assunto);

            //assert
            Assert.NotNull(recado);
            Assert.Equal(outroRecadoExistente.Destinatario, recado.Destinatario);
            Assert.Equal(outroRecadoExistente.Remetente, recado.Remetente);
            Assert.Equal(outroRecadoExistente.Assunto, recado.Assunto);
            Assert.Equal(outroRecadoExistente.Id, recado.Id);
            Assert.Equal(outroRecadoExistente.Mensagem, recado.Mensagem);
        }

        [Fact]
        public void ObterRecadoParaAgrupamentoRetornarNullUmRecadoQuandoExistirMasDataCriacaoPassouDeSeisMeses()
        {
            //arrange
            var recadoExistente = new Recado("Fulano", "Ciclano", "Primeiro recado", "Msg");
            AlterarPropriedadeComReflection(recadoExistente, nameof(Recado.DataCriacao), DateTimeOffset.UtcNow.AddDays(-7 * 30));
            _sut.Adicionar(recadoExistente);
            _ctx.DetachAllEntries();
            var recadoInserir = new Recado("Ciclano", "Fulano", "respondendo: Primeiro recado", "");

            //act
            var recado = _sut.ObterRecadoParaAgrupamento(recadoInserir.Remetente, recadoInserir.Destinatario, recadoInserir.Assunto);

            //assert
            Assert.Null(recado);
        }

        private static void AlterarPropriedadeComReflection<T, TValue>(T objeto, string propriedade, TValue valor)
        {
            var type = typeof(T);
            var fieldName = $"<{nameof(Recado.DataCriacao)}>k__BackingField";

            var prop = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic) ??
                type.BaseType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            
            prop.SetValue(objeto, valor);
        }

        public void Dispose()
        {
            _ctx.DetachAllEntries();
            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}
