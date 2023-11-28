namespace HandlingErrors.Web.Infra;

public interface IODataQuery<T>
{
    IQueryable Apply(IQueryable<T> queryable);
}