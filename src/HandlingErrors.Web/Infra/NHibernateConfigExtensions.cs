using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using HandlingErrors.Data;
using Microsoft.Extensions.DependencyInjection;

namespace HandlingErrors.Web.Infra
{
    public static class NHibernateConfigExtensions
    {
        public static void AddNHibernate(this IServiceCollection services, string connectionString)
        {
            var sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                .ConnectionString(c => c.Is(connectionString)))
                .Mappings(m => m.FluentMappings.Add<RecadoMap>())
                .BuildSessionFactory();

            services.AddSingleton<SqlDebugOutputInterceptor>();
            services.AddSingleton(sessionFactory);
            services.AddScoped(factory => sessionFactory.WithOptions().Interceptor(factory.GetService<SqlDebugOutputInterceptor>()).OpenSession());
        }
    }
}
