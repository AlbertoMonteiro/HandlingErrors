using Refit;

namespace HandlingErrors.Core.Services;

/// <summary>
/// API que fornece dados de clima
/// </summary>
/// <C4Technology>ASP.NET Core</C4Technology>
/// <C4Owner>AKS</C4Owner>
/// <C4Tags>kubernetes,cloud</C4Tags>
/// <C4RelationshipLabel>Uses</C4RelationshipLabel>
/// <C4RelationshipProtocol>HTTPS</C4RelationshipProtocol>
public interface IWeatherForecastService
{
    [Get("weather")]
    Task<ApiResponse<string>> GetWeather();
}

/// <summary>
/// Uma dependência genérica para exemplo
/// </summary>
/// <C4Technology>Java</C4Technology>
/// <C4Owner>AKS</C4Owner>
/// <C4Tags>kubernetes,cloud</C4Tags>
/// <C4RelationshipLabel>Uses</C4RelationshipLabel>
/// <C4RelationshipProtocol>gRPC</C4RelationshipProtocol>
public interface IGenericDependency
{
    Task<string> GetWeather();
}
