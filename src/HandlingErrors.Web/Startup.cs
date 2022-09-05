using HandlingErrors.Data;
using HandlingErrors.IoC;
using HandlingErrors.Web.Infra;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace HandlingErrors.Web;

public class Startup
{
    private const string APP_NAME = "HandlingErrors Recados App";

    public Startup(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers(op =>
            {
                foreach (var formatter in op.OutputFormatters.OfType<ODataOutputFormatter>().Where(it => it.SupportedMediaTypes.Count == 0))
                    formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/odata"));

                foreach (var formatter in op.InputFormatters.OfType<ODataInputFormatter>().Where(it => it.SupportedMediaTypes.Count == 0))
                    formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/odata"));
            })
            .AddJsonOptions(op =>
            {
                op.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                op.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .AddOData(options => options.Select().Filter().OrderBy().SetMaxTop(30));

        var connectionString = Configuration["ConnectionStrings:DefaultConnection"];
        if (Configuration.GetValue<bool>("useInMemory"))
            services.AddDbContext<HandlingErrorsContext>(options => options.UseInMemoryDatabase("HandlingErrors"), ServiceLifetime.Scoped);
        else
            services.AddDbContext<HandlingErrorsContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Scoped);

        services.AddServices();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = APP_NAME, Version = "v1" });
            c.OperationFilter<SwaggerAddODataField>();
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
