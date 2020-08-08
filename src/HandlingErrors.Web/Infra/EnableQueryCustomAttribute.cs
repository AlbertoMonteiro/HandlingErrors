using System.Collections;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HandlingErrors.Web.Infra
{
    public sealed class EnableQueryCustomAttribute : EnableQueryAttribute
    {
        public EnableQueryCustomAttribute()
        {
            PageSize = 10;
            HandleNullPropagation = HandleNullPropagationOption.False;
        }

        public override void ValidateQuery(HttpRequest request, ODataQueryOptions queryOptions)
        {
            base.ValidateQuery(request, queryOptions);
            request.HttpContext.Items.Add(nameof(ODataQueryOptions), queryOptions);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
                return;

            base.OnActionExecuted(context);

            var oDataFeature = context.HttpContext.ODataFeature();
            if (oDataFeature.TotalCount.HasValue && context.Result is ObjectResult obj && obj.Value is IQueryable queryable)
                context.Result = new ObjectResult(new PaginatedResult(oDataFeature.TotalCount, queryable)) { StatusCode = 200 };
        }
    }

    public class PaginatedResult
    {
        public PaginatedResult(long? count, IEnumerable allItems)
        {
            Count = count;
            Items = allItems;
        }

        public long? Count { get; set; }
        public IEnumerable Items { get; set; }
    }
}
