using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Xunit;

namespace HandlingErrors.Data.Tests.Infra;

public sealed class DatabaseFixture : IAsyncLifetime
{
    readonly MsSqlContainer container = new MsSqlBuilder().Build();

    public HandlingErrorsContext Context { get; private set; }
    public IMapper Mapper { get; private set; }

    public async Task InitializeAsync()
    {
        await container.StartAsync();
         
         var options = new DbContextOptionsBuilder<HandlingErrorsContext>();
        options.UseSqlServer(container.GetConnectionString());
        Context = new HandlingErrorsContext(options.Options);
        Context.Database.EnsureCreated();
        Mapper = GetMapper();
    }

    public Task DisposeAsync() 
        => container.DisposeAsync().AsTask();

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
