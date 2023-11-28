using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.Edm;

namespace HandlingErrors.Web.Infra;
public class ODataQuery<T> : IODataQuery<T>
{
    private readonly ODataQueryOptions<T> _options;

    public ODataQuery(IEdmModel edmModel, IHttpContextAccessor httpContext)
        => _options = new ODataQueryOptions<T>(new ODataQueryContext(edmModel, typeof(T), null), httpContext.HttpContext.Request);

    public IQueryable Apply(IQueryable<T> queryable)
        => _options.ApplyTo(queryable);
}
