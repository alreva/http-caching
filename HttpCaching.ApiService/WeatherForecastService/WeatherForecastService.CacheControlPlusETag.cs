using System.Globalization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HttpCaching.ApiService.WeatherForecastService;

partial class WeatherForecastService
{
    public async Task<Results<
            Ok<WeatherForecast[]>,
            StatusCodeHttpResult>
        > CacheControlPlusETag(HttpContext ctx)
    {
        if (!TryGetIfNoneMatchFromHeaders(ctx.Request.Headers, out var eTag))
        {
            return await NoCacheWithCcMustRevalidatePlusEtag(ctx);
        }
        
        if (weatherOptions.ETag != eTag)
        {
            return await NoCacheWithCcMustRevalidatePlusEtag(ctx);
        }

        return EmptyResultWith304NotModifiedStatus(ctx);
    }

    #region Private methods
    
    private async Task<Results<Ok<WeatherForecast[]>, StatusCodeHttpResult>> NoCacheWithCcMustRevalidatePlusEtag(HttpContext ctx)
    {
        ctx.Response.Headers.CacheControl = $"public, max-age={weatherOptions.MaxAge}, must-revalidate";
        ctx.Response.Headers.ETag = weatherOptions.ETag;
        return await NoCache();
    }

    #endregion
}