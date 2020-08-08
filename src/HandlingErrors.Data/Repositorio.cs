using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using HandlingErrors.Core.Modelos;
using HandlingErrors.Core.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Reflection;

namespace HandlingErrors.Data
{
    public class Repositorio<T> : IRepositorio<T>
        where T : Entidade
    {
        protected readonly HandlingErrorsContext _contexto;
        protected readonly DbSet<T> _currentSet;
        protected readonly IConfigurationProvider _mapperConfigProvider;

        protected event AntesDeAlterarDelegate AntesDeDeletar;
        protected event AntesDeAlterarDelegate AntesDeAtualizar;

        public delegate void AntesDeAlterarDelegate(ref EntityEntry<T> entity);

        public Repositorio(HandlingErrorsContext ctx, IMapper mapper)
        {
            _contexto = ctx;
            _currentSet = _contexto.Set<T>();
            _mapperConfigProvider = mapper.ConfigurationProvider;
        }

        public T ObterPorId(long id)
            => _currentSet.FirstOrDefault(e => e.Id == id);

        public T ObterPorIdAsNoTrack(long id)
          => _currentSet.AsNoTracking().FirstOrDefault(e => e.Id == id);

        public TViewModel ObterProjetado<TViewModel>(long id)
            => _currentSet.Where(e => e.Id == id).UseAsDataSource(_mapperConfigProvider).For<TViewModel>().FirstOrDefault();

        public IQueryable<T> ObterTodos()
            => _currentSet;

        public IQueryable<TViewModel> ObterTodosProjetado<TViewModel>(object parameters = null)
            => _currentSet.UseAsDataSource(_mapperConfigProvider).For<TViewModel>(parameters);

        public long Adicionar(T entity)
        {
            _currentSet.Add(entity);
            _contexto.SaveChanges();
            return entity.Id;
        }

        public void Remover(long id)
        {
            var entityEntry = _contexto.Entry(CriarInstancia<T>());
            entityEntry.Property<long>(nameof(Entidade.Id)).CurrentValue = id;
            entityEntry.State = EntityState.Deleted;
            AntesDeDeletar?.Invoke(ref entityEntry);
            _contexto.SaveChanges();
        }

        public void Atualizar(T entity)
        {
            var entityEntry = _contexto.Entry(entity);
            AtualizarSemMudarDataCriacao(in entityEntry);
            AntesDeAtualizar?.Invoke(ref entityEntry);
            _contexto.SaveChanges();
        }

        private static TEntity CriarInstancia<TEntity>()
            => (TEntity)Activator.CreateInstance(typeof(TEntity), BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, null);

        protected static void AtualizarSemMudarDataCriacao<TEntity>(in EntityEntry<TEntity> entityEntry)
            where TEntity : Entidade
        {
            entityEntry.State = EntityState.Modified;
            entityEntry.Property(t => t.DataCriacao).IsModified = false;
        }

        public bool Existe(long id) => _currentSet.Any(e => e.Id == id);
    }
}
