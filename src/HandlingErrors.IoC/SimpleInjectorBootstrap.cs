using AutoMapper;
using AutoMapper.Configuration;
using AutoMapper.Extensions.ExpressionMapping;
using HandlingErrors.Core.Modelos;
using HandlingErrors.Core.Repositorios;
using HandlingErrors.Core.RequestHandlers.Pipelines;
using HandlingErrors.Data;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HandlingErrors.IoC
{
    public static class SimpleInjectorBootstrap
    {
        private static readonly Type _repoType = typeof(Repositorio<>);
        private static readonly Type _entityType = typeof(Entidade);
        private static readonly Type _profileType = typeof(Profile);

        public static void Initialize(Container container)
        {
            container.Register(typeof(IRepositorio<>), _repoType, Lifestyle.Scoped);

            var assembliesToScan = _repoType.GetTypeInfo().Assembly;
            var typesToRegister = assembliesToScan.ExportedTypes.Select(t => t.GetTypeInfo());

            var registrations = from type in typesToRegister
                                let @interface = type.ImplementedInterfaces.FirstOrDefault(inter => inter.Name == $"I{type.Name}")
                                where @interface != null && type.IsClass && !type.IsGenericType
                                select (@interface, type.AsType());

            foreach (var (@interface, @class) in registrations)
                container.Register(@interface, @class, Lifestyle.Scoped);

            ConfigureValidators(container);

            container.RegisterSingleton(() => GetMapper(container));

            ConfigureMediatR(container);
        }

        private static void ConfigureValidators(Container container)
        {
            var baseType = typeof(AbstractValidator<>);
            var validatorTypes = _entityType.Assembly.ExportedTypes
                .Where(t => t.IsClass && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == baseType)
                .ToArray();

            foreach (var type in validatorTypes)
                container.Register(type.BaseType, type, Lifestyle.Scoped);
        }

        private static void ConfigureMediatR(Container container)
        {
            var assemblies = GetAssemblies().ToArray();
            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(typeof(IRequestHandler<,>), assemblies);

            // we have to do this because by default, generic type definitions (such as the Constrained Notification Handler) won't be registered
            var notificationHandlerTypes = container.GetTypesToRegister(typeof(INotificationHandler<>), assemblies, new TypesToRegisterOptions
            {
                IncludeGenericTypeDefinitions = true,
                IncludeComposites = false,
            });
            container.Collection.Register(typeof(INotificationHandler<>), notificationHandlerTypes);

            //Pipeline
            container.Collection.Register(typeof(IPipelineBehavior<,>), new[]
            {
                typeof (RequestPreProcessorBehavior<,>),
                typeof (RequestPostProcessorBehavior<,>),
                // The order does matter, dont change the ExceptionPipelineBehavior and ValidationPipelineBehavior positions
                typeof (ExceptionPipelineBehavior<,>),
                typeof (ValidationPipelineBehavior<,>),
            });
            container.Collection.Register(typeof(IRequestPreProcessor<>));
            container.Collection.Register(typeof(IRequestPostProcessor<,>));

            container.Register(() => new ServiceFactory(container.GetInstance), Lifestyle.Singleton);
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(ExceptionPipelineBehavior<,>).GetTypeInfo().Assembly;
        }

        private static IMapper GetMapper(Container container)
        {
            var mce = new MapperConfigurationExpression();
            mce.ConstructServicesUsing(container.GetInstance);

            var profiles = _repoType.Assembly.ExportedTypes
                .Where(type => !type.IsAbstract && _profileType.IsAssignableFrom(type))
                .Select(x => Activator.CreateInstance(x))
                .Cast<Profile>();

            mce.AddProfiles(profiles);
            mce.AddExpressionMapping();

            var mc = new MapperConfiguration(mce);
            mc.AssertConfigurationIsValid();

            return new Mapper(mc, t => container.GetInstance(t));
        }
    }
}
