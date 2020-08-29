using System;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HandlingErrors.Core.Modelos;
using HandlingErrors.Core.Repositorios;
using NHibernate;

namespace HandlingErrors.Data
{
    public class RecadoRepositorioNHibernate : IRecadoRepositorioNHibernate
    {
        const int SEIS_MESES_EM_DIAS = 30 * 6;
        private static readonly string[] _textosParaIgnorar = new[] { "referente:", "respondendo:", "complementando:" };
        private readonly ISession _ctx;
        private readonly IMapper _mapper;

        public RecadoRepositorioNHibernate(ISession ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public TViewModel ObterProjetado<TViewModel>(long id)
        {
            return _ctx.Query<Recado>().Where(x => x.Id == id).ProjectTo<TViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
        }

        public IQueryable<TViewModel> ObterTodosProjetado<TViewModel>(object parametros = null)
        {
            return _ctx.Query<Recado>().ProjectTo<TViewModel>(_mapper.ConfigurationProvider, parametros);
        }

        public Recado ObterRecadoParaAgrupamento(string rementente, string destinatario, string assunto)
        {
            var hoje = DateTimeOffset.UtcNow.Date;
            var textoRemover = _textosParaIgnorar.FirstOrDefault(ignorar => assunto.IndexOf(ignorar) >= 0);
            if (textoRemover != null)
            {
                assunto = assunto.Substring(textoRemover.Length).Trim();
            }

            return _ctx.Query<Recado>()
                .OrderByDescending(r => r.DataCriacao)
                .FirstOrDefault(r => (r.Remetente == rementente || r.Remetente == destinatario)
                && r.Assunto.Replace("referente:", "").Replace("respondendo:", "").Replace("complementando:", "").Trim() == assunto
                && (r.DataCriacao.Date - hoje).TotalDays <= SEIS_MESES_EM_DIAS);
        }

        public long Adicionar(Recado entidade)
        {
            throw new NotImplementedException();
        }

        public void Atualizar(Recado entidade)
        {
            throw new NotImplementedException();
        }

        public bool Existe(long id)
        {
            throw new NotImplementedException();
        }

        public Recado ObterPorId(long id)
        {
            throw new NotImplementedException();
        }

        public Recado ObterPorIdAsNoTrack(long id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Recado> ObterTodos()
        {
            throw new NotImplementedException();
        }

        public void Remover(long id)
        {
            throw new NotImplementedException();
        }
    }
}
