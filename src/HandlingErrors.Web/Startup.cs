using HandlingErrors.Data;
using HandlingErrors.IoC;
using HandlingErrors.Web.Endpoints;
using HandlingErrors.Web.Infra;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace HandlingErrors.Web;

public class Startup
{
    private const string APP_NAME = "HandlingErrors Recados App";

    public Startup(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOData();

        var connectionString = Configuration["ConnectionStrings:DefaultConnection"];
        if (Configuration.GetValue<bool>("useInMemory"))
            services.AddDbContext<HandlingErrorsContext>(options => options.UseInMemoryDatabase("HandlingErrors"), ServiceLifetime.Scoped);
        else
            services.AddDbContext<HandlingErrorsContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Scoped);

        services.AddServices();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = APP_NAME, Version = "v1" });
            c.OperationFilter<SwaggerAddODataField>();
        });
    }

    public void Configure<T>(T app, IWebHostEnvironment env)
        where T : IApplicationBuilder, IEndpointRouteBuilder
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<HandlingErrorsContext>().Database.EnsureCreated();
        }

        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseRouting();

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", APP_NAME));

        RecadosEndpoints.MapGet(app);
        RecadosEndpoints.MapPost(app);
    }
}
