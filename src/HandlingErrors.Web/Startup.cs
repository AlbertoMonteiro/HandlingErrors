using Aurelia.DotNet;
using HandlingErrors.Data;
using HandlingErrors.IoC;
using HandlingErrors.Web.Infra;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Linq;
using System.Text.Json.Serialization;

namespace HandlingErrors.Web
{
    public class Startup
    {
        private const string APP_NAME = "HandlingErrors Recados App";
        private readonly Container _container = new Container();

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            InitializeContainer();
            services.AddControllers();
            services.AddOData();

            services.AddMvcCore(op =>
            {
                foreach (var formatter in op.OutputFormatters.OfType<ODataOutputFormatter>().Where(it => it.SupportedMediaTypes.Count == 0))
                    formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/odata"));

                foreach (var formatter in op.InputFormatters.OfType<ODataInputFormatter>().Where(it => it.SupportedMediaTypes.Count == 0))
                    formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/odata"));
            })
            .AddJsonOptions(op =>
            {
                op.JsonSerializerOptions.IgnoreNullValues = true;
                op.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            var connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            if (Configuration.GetValue<bool>("useInMemory"))
                services.AddDbContext<HandlingErrorsContext>(options => options.UseInMemoryDatabase("HandlingErrors"), ServiceLifetime.Scoped);
            else
                services.AddDbContext<HandlingErrorsContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Scoped);

            services.AddSimpleInjector(_container, c => c.AddAspNetCore().AddControllerActivation());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = APP_NAME, Version = "v1" });
                c.OperationFilter<SwaggerAddODataField>();
            });

            services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/dist");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSimpleInjector(_container);
            _container.Verify();

            using var scope = AsyncScopedLifestyle.BeginScope(_container);
            scope.GetService<HandlingErrorsContext>().Database.EnsureCreated();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", APP_NAME);
            });

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.EnableDependencyInjection();
                endpoints.Select().Filter().OrderBy().Count().MaxTop(30);
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                    spa.UseAureliaCliServer();
            });
        }

        private void InitializeContainer()
        {
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            SimpleInjectorBootstrap.Initialize(_container);
        }
    }
}
