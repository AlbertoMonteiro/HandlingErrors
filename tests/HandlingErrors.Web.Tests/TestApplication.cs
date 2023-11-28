using HandlingErrors.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HandlingErrors.Web.Tests;
internal class TestApplication : WebApplicationFactory<Program>
{
    private readonly string _environment;

    public TestApplication(string environment = "Development")
        => _environment = environment;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var mediator = NSubstitute.Substitute.For<IMediator>();
        builder.ConfigureAppConfiguration((x, _) => x.Configuration["urls"] = "*");
        builder.ConfigureServices(services =>
        {
            services.AddTransient(_ => new HandlingErrorsContext(new DbContextOptionsBuilder<HandlingErrorsContext>().UseInMemoryDatabase("HandlingErrorsRecados").Options));
            services.AddTransient(_ => mediator);
        });
        builder.UseEnvironment(_environment);
        return base.CreateHost(builder);
    }
}
