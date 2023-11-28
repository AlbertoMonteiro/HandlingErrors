using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using FluentValidation;
using HandlingErrors.Core.Modelos;
using HandlingErrors.Core.Repositorios;
using HandlingErrors.Core.RequestHandlers.Pipelines;
using HandlingErrors.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HandlingErrors.IoC;

public static class AppServicesBootstrap
{
    private static readonly Type _repoType = typeof(Repositorio<>);
    private static readonly Type _entityType = typeof(Entidade);
    private static readonly Type _profileType = typeof(Profile);

    public static void AddServices(this IServiceCollection container)
    {
        container.AddScoped(typeof(IRepositorio<>), _repoType);

        var assembliesToScan = _repoType.GetTypeInfo().Assembly;
        var typesToRegister = assembliesToScan.ExportedTypes.Select(t => t.GetTypeInfo());

        var registrations = from type in typesToRegister
                            let @interface = type.ImplementedInterfaces.FirstOrDefault(inter => inter.Name == $"I{type.Name}")
                            where @interface != null && type.IsClass && !type.IsGenericType
                            select (@interface, type.AsType());

        foreach (var (@interface, @class) in registrations)
            container.AddScoped(@interface, @class);

        ConfigureValidators(container);

        container.AddSingleton((sp) => GetMapper(sp));

        ConfigureMediatR(container);
    }

    private static void ConfigureValidators(IServiceCollection container)
    {
        var baseType = typeof(AbstractValidator<>);
        var validatorTypes = _entityType.Assembly.ExportedTypes
            .Where(t => t.IsClass && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == baseType)
            .ToArray();

        foreach (var type in validatorTypes)
            container.AddScoped(type.BaseType, type);
    }

    private static void ConfigureMediatR(IServiceCollection container)
    {
        var assemblies = GetAssemblies().ToArray();
        container.AddMediatR(x => x
                    .RegisterServicesFromAssemblies(typeof(ExceptionPipelineBehavior<,>).Assembly)
                    .AddOpenBehavior(typeof(ExceptionPipelineBehavior<,>))
                    .AddOpenBehavior(typeof(ValidationPipelineBehavior<,>)));
    }

    private static IEnumerable<Assembly> GetAssemblies()
    {
        yield return typeof(ExceptionPipelineBehavior<,>).GetTypeInfo().Assembly;
    }

    private static IMapper GetMapper(IServiceProvider container)
    {
        var mce = new MapperConfigurationExpression();
        mce.ConstructServicesUsing(container.GetService);

        var profiles = _repoType.Assembly.ExportedTypes
            .Where(type => !type.IsAbstract && _profileType.IsAssignableFrom(type))
            .Select(x => Activator.CreateInstance(x))
            .Cast<Profile>();

        mce.AddProfiles(profiles);
        mce.AddExpressionMapping();

        var mc = new MapperConfiguration(mce);
        mc.AssertConfigurationIsValid();

        return new Mapper(mc, t => container.GetService(t));
    }
}
