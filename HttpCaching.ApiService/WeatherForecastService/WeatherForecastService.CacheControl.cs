using System.Globalization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Primitives;

namespace HttpCaching.ApiService.WeatherForecastService;

partial class WeatherForecastService
{
    public async Task<Results<
            Ok<WeatherForecast[]>,
            StatusCodeHttpResult>
        > CacheControl(HttpContext ctx)
    {
        return await NoCacheWithCacheControlMustRevalidate(ctx);
    }   

    #region Private methods

    private async Task<Results<Ok<WeatherForecast[]>, StatusCodeHttpResult>> NoCacheWithCacheControlMustRevalidate(HttpContext ctx)
    {
        ctx.Response.Headers.CacheControl = $"public, max-age={weatherOptions.MaxAge}, must-revalidate";
        return await NoCache();
    }

    #endregion
}