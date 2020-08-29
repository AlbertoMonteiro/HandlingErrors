using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.SqlCommand;

namespace HandlingErrors.Web.Infra
{
    public sealed class SqlDebugOutputInterceptor : EmptyInterceptor
    {
        private readonly ILogger<SqlDebugOutputInterceptor> _logger;

        public SqlDebugOutputInterceptor(ILogger<SqlDebugOutputInterceptor> logger) 
            => _logger = logger;

        public override SqlString OnPrepareStatement(SqlString sql)
        {
            _logger.LogInformation(sql.ToString());

            return base.OnPrepareStatement(sql);
        }
    }
}
