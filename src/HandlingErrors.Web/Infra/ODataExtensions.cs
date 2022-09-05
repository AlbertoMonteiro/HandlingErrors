using HandlingErrors.Shared.ViewModels;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using System.Reflection;

namespace HandlingErrors.Web.Infra;

public static class ODataExtensions
{
    public static void AddOData(this IServiceCollection services)
    {
        var odataMethod = typeof(ODataMvcCoreBuilderExtensions).GetMethod("AddODataCore", BindingFlags.NonPublic | BindingFlags.Static);
        odataMethod?.Invoke(null, null);

        var odataBuilder = new ODataConventionModelBuilder();
        odataBuilder.EntitySet<RecadoViewModel>("Recado");

        services.AddSingleton(odataBuilder.GetEdmModel());

        services.AddHttpContextAccessor();
        services.Configure<ODataOptions>(options =>
        {
            options.RouteOptions.EnableQualifiedOperationCall = false;
            options.EnableAttributeRouting = false;
            options.Select().Filter().OrderBy();
        });
        services.AddScoped(typeof(IODataQuery<>), typeof(ODataQuery<>));
    }
}
