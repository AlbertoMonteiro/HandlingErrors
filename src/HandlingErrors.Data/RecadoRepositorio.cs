using AutoMapper;
using HandlingErrors.Core.Modelos;
using HandlingErrors.Core.Repositorios;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using static Microsoft.EntityFrameworkCore.EF;

namespace HandlingErrors.Data
{
    public class RecadoRepositorio : Repositorio<Recado>, IRecadoRepositorio
    {
        const int SEIS_MESES_EM_DIAS = 30 * 6;
        private static readonly string[] _textosParaIgnorar = new[] { "referente:", "respondendo:", "complementando:" };

        public RecadoRepositorio(HandlingErrorsContext ctx, IMapper mapper)
            : base(ctx, mapper)
        {
        }

        public Recado ObterRecadoParaAgrupamento(string rementente, string destinatario, string assunto)
        {
            var hoje = DateTimeOffset.UtcNow.Date;
            var textoRemover = _textosParaIgnorar.FirstOrDefault(ignorar => assunto.IndexOf(ignorar) >= 0);
            if (textoRemover != null)
                assunto = assunto.Substring(textoRemover.Length).Trim();

            return _contexto.Recados
                .OrderByDescending(r => r.DataCriacao)
                .FirstOrDefault(r => (r.Remetente == rementente || r.Remetente == destinatario)
                && r.Assunto.Replace("referente:", "").Replace("respondendo:", "").Replace("complementando:", "").Trim() == assunto
                && Functions.DateDiffDay(r.DataCriacao.Date, hoje) <= SEIS_MESES_EM_DIAS);
        }
    }
}
