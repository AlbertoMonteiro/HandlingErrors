using AutoMapper;
using AutoMapper.Configuration;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HandlingErrors.Data.Tests.Infra
{
    public sealed class DatabaseFixture
    {
        private const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=HandlingErrorsRecadosTest;Trusted_Connection=True;";
        public HandlingErrorsContext Context { get; }
        public IMapper Mapper { get; }

        public DatabaseFixture()
        {
            var options = new DbContextOptionsBuilder<HandlingErrorsContext>();
            options.UseSqlServer(ConnectionString);
            Context = new HandlingErrorsContext(options.Options);
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
            Mapper = GetMapper();
        }

        public static IMapper GetMapper()
        {
            var mce = new MapperConfigurationExpression();
            mce.ConstructServicesUsing(Activator.CreateInstance);

            var profileType = typeof(Profile);
            var profiles = typeof(Repositorio<>).Assembly.ExportedTypes
                .Where(type => !type.IsAbstract && profileType.IsAssignableFrom(type))
                .Select(x => Activator.CreateInstance(x))
                .Cast<Profile>()
                .ToArray();

            mce.AddProfiles(profiles);
            mce.AddExpressionMapping();

            var mc = new MapperConfiguration(mce);
            mc.AssertConfigurationIsValid();

            return new Mapper(mc);
        }
    }
}
